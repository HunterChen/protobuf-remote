
#include "ProtoBufRemote/SocketRpcChannel.h"

#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/RpcMessage.pb.h"


namespace ProtoBufRemote {

SocketRpcChannel::SocketRpcChannel(RpcController* controller, SOCKET socket)
	: RpcChannel(controller), m_socket(socket)
{
	m_sendEvent = WSACreateEvent();
	m_terminateEvent = WSACreateEvent();
}

SocketRpcChannel::~SocketRpcChannel()
{
	CloseAndJoin();
	WSACloseEvent(m_sendEvent);
	WSACloseEvent(m_terminateEvent);
}

void SocketRpcChannel::Start()
{
	m_thread = boost::thread(&SocketRpcChannel::Run, this);
}

void SocketRpcChannel::CloseAndJoin()
{
	WSASetEvent(m_terminateEvent);
	m_thread.join();
}

unsigned int SocketRpcChannel::GetAndClearBytesRead()
{
	return _InterlockedExchange(reinterpret_cast<LONG*>(&m_bytesRead), 0);
}

unsigned int SocketRpcChannel::GetAndClearBytesWritten()
{
	return _InterlockedExchange(reinterpret_cast<LONG*>(&m_bytesWritten), 0);
}

void SocketRpcChannel::Send(const RpcMessage& message)
{
	QueuedMessageData data;
	unsigned int messageSize = message.ByteSize();
	data.m_size = messageSize + sizeof(int);
	data.m_data = new char[data.m_size];
	*reinterpret_cast<unsigned int*>(data.m_data) = messageSize;
	message.SerializeToArray(data.m_data+sizeof(int), messageSize);

	boost::lock_guard<boost::mutex> lock(m_sendMutex);
	m_sendMessages.push(data);
	WSASetEvent(m_sendEvent);
}

void SocketRpcChannel::Run()
{
	WSAEVENT selectEvent = WSACreateEvent();
	WSAEventSelect(m_socket, selectEvent, FD_READ|FD_WRITE);

	bool isSending = false;
	unsigned int sendPos = 0;
	QueuedMessageData currentSendMessage;

	bool isReceiving = false;
	unsigned int receivePos = 0;
	unsigned int receiveSize = sizeof(int);
	std::vector<char> receiveBuffer;
	receiveBuffer.resize(1024);
	const unsigned int maxAllowedMessageSize = 1024*1024;

	WSAEVENT events[3] = { m_terminateEvent, selectEvent, m_sendEvent };
	bool isTerminated = false;
	while (!isTerminated)
	{
		int waitResult = WSAWaitForMultipleEvents(isSending ? 2 : 3, events, FALSE, WSA_INFINITE, FALSE);
		if (waitResult == 0)
			break;

		bool isSendReady = false;
		bool isReceiveReady = false;
		if (waitResult == 2)
		{
			assert(!isSending);
			boost::lock_guard<boost::mutex> lock(m_sendMutex);
			if (!m_sendMessages.empty())
			{
				currentSendMessage = m_sendMessages.front();
				m_sendMessages.pop();
				isSending = true;
				isSendReady = true; //send immediately
				sendPos = 0;
			}
		}
		else
		{
			WSANETWORKEVENTS networkEvents;
			int result = WSAEnumNetworkEvents(m_socket, selectEvent, &networkEvents);
			isSendReady = (networkEvents.lNetworkEvents & FD_WRITE) ? true : false;
			isReceiveReady = (networkEvents.lNetworkEvents & FD_READ) ? true : false;
		}

		while (isSending && isSendReady && !isTerminated)
		{
			int bytesSent = send(m_socket, &currentSendMessage.m_data[sendPos], currentSendMessage.m_size-sendPos, 0);
			if (bytesSent == SOCKET_ERROR)
			{
				int error = WSAGetLastError();
				if (error == WSAEWOULDBLOCK)
					isSendReady = false;
				else
					isTerminated = true;
			}
			else
			{
				sendPos += bytesSent;
				_InterlockedExchangeAdd(reinterpret_cast<LONG*>(&m_bytesWritten), bytesSent);

				if (sendPos >= currentSendMessage.m_size)
				{
					delete[] currentSendMessage.m_data;

					boost::lock_guard<boost::mutex> lock(m_sendMutex);
					if (m_sendMessages.empty())
					{
						WSAResetEvent(m_sendEvent);
						isSending = false;
					}
					else
					{
						currentSendMessage = m_sendMessages.front();
						m_sendMessages.pop();
						sendPos = 0;
					}
				}
			}
		}

		while (isReceiveReady && !isTerminated)
		{
			int bytesReceived = recv(m_socket, &receiveBuffer[receivePos], receiveSize-receivePos, 0);
			if (bytesReceived == 0)
			{
				isTerminated = true;
			}
			else if (bytesReceived == SOCKET_ERROR)
			{
				int error = WSAGetLastError();
				if (error == WSAEWOULDBLOCK)
					isReceiveReady = false;
				else
					isTerminated = true;
			}
			else
			{
				receivePos += bytesReceived;
				_InterlockedExchangeAdd(reinterpret_cast<LONG*>(&m_bytesRead), bytesReceived);

				if (receivePos >= receiveSize)
				{
					if (isReceiving)
					{
						RpcMessage message;
						message.ParseFromArray(&receiveBuffer[0], receiveSize);
						m_controller->Receive(message);

						isReceiving = false;
						receiveSize = sizeof(int);
						receivePos = 0;
					}
					else
					{
						isReceiving = true;
						receiveSize = *reinterpret_cast<unsigned int*>(&receiveBuffer[0]);
						receivePos = 0;

						if (receiveSize > maxAllowedMessageSize)
						{
							assert(false);
							break; //better to abort rather than allocate based on bad data
						}
						if (receiveBuffer.size() < receiveSize)
							receiveBuffer.resize(receiveSize);
					}
				}
			}
		}
	}

	closesocket(m_socket);
	m_socket = INVALID_SOCKET;
	WSACloseEvent(selectEvent);
}

}

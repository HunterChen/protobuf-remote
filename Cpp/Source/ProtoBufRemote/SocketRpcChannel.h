#ifndef PROTOBUFREMOTE_SOCKETRPCCHANNEL_H
#define PROTOBUFREMOTE_SOCKETRPCCHANNEL_H 1

#include <winsock2.h>
#include <queue>
#include <boost/thread.hpp>
#include "ProtoBufRemote/RpcChannel.h"

namespace ProtoBufRemote {

class SocketRpcChannel : RpcChannel
{
public:
	SocketRpcChannel(RpcController* controller, SOCKET socket);
	virtual ~SocketRpcChannel();

	void Start();

	void CloseAndJoin();

	virtual void Send(const RpcMessage& message);

	unsigned int GetAndClearBytesRead();

	unsigned int GetAndClearBytesWritten();

private:
	void Run();

	SOCKET m_socket;
	boost::thread m_thread;
	WSAEVENT m_sendEvent;
	WSAEVENT m_terminateEvent;

	boost::mutex m_sendMutex;

	struct QueuedMessageData
	{
		unsigned int m_size;
		char* m_data;
	};
	std::queue<QueuedMessageData> m_sendMessages;

	unsigned int m_bytesRead;
	unsigned int m_bytesWritten;
};

}

#endif

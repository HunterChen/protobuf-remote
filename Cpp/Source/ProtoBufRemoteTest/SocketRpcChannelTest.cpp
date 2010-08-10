
#include <gtest/gtest.h>

#include <ws2tcpip.h>
#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/RpcMessage.pb.h"
#include "ProtoBufRemote/SocketRpcChannel.h"


using ProtoBufRemote::RpcController;
using ProtoBufRemote::RpcMessage;
using ProtoBufRemote::SocketRpcChannel;

namespace {
	SocketRpcChannel* g_serverChannel;
}

void ServerThreadRun()
{
	ADDRINFO hints;
	ADDRINFO* addrResult;
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_INET;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	hints.ai_flags = AI_PASSIVE;
	int result = getaddrinfo(NULL, "43210", &hints, &addrResult);

	SOCKET listenSocket = socket(addrResult->ai_family, addrResult->ai_socktype, addrResult->ai_protocol);

	result = bind(listenSocket, addrResult->ai_addr, (int)addrResult->ai_addrlen);

	result = listen(listenSocket, SOMAXCONN);

	SOCKET clientSocket = accept(listenSocket, NULL, NULL);
	
	g_serverChannel = new SocketRpcChannel(NULL, clientSocket);
	g_serverChannel->Start();

	freeaddrinfo(addrResult);
	closesocket(listenSocket);
}

class DummyRpcController : public RpcController
{
public:
	DummyRpcController()
		: m_isResultOk(true)
	{
		m_doneEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
	}
	~DummyRpcController()
	{
		CloseHandle(m_doneEvent);
	}
	virtual void Receive(const RpcMessage& message)
	{
		if (message.id() != 42)
			m_isResultOk = false;
		const RpcMessage::Call& callMessage = message.call_message();
		if (callMessage.service() != "TestService" || callMessage.method() != "TestMethod")
			m_isResultOk = false;
		if (callMessage.expects_result())
			m_isResultOk = false;
		SetEvent(m_doneEvent);
	}
	bool WaitForResult()
	{
		WaitForSingleObject(m_doneEvent, INFINITE);
		return m_isResultOk;
	}
private:
	HANDLE m_doneEvent;
	bool m_isResultOk;
};

TEST(SocketRpcChannelTest, SendReceive)
{
	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2,2), &wsaData);

	boost::thread serverThread(ServerThreadRun);

	ADDRINFO hints;
	ADDRINFO* addrResult;
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	result = getaddrinfo("127.0.0.1", "43210", &hints, &addrResult);

	SOCKET connectSocket = socket(addrResult->ai_family, addrResult->ai_socktype, addrResult->ai_protocol);
	result = connect(connectSocket, addrResult->ai_addr, (int)addrResult->ai_addrlen);

	serverThread.join();

	DummyRpcController dummyController;
	SocketRpcChannel clientChannel(&dummyController, connectSocket);
	clientChannel.Start();

	RpcMessage message;
	message.set_id(42);
	RpcMessage::Call* callMessage = message.mutable_call_message();
	callMessage->set_service("TestService");
	callMessage->set_method("TestMethod");
	callMessage->set_expects_result(false);
	g_serverChannel->Send(message);

	EXPECT_TRUE(dummyController.WaitForResult());

	clientChannel.CloseAndJoin();
	g_serverChannel->CloseAndJoin();
	delete g_serverChannel;

	result = WSACleanup();
}

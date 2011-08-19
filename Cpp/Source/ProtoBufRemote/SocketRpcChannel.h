#ifndef PROTOBUFREMOTE_SOCKETRPCCHANNEL_H
#define PROTOBUFREMOTE_SOCKETRPCCHANNEL_H 1

#include <winsock2.h>
#include <queue>
#include "ProtoBufRemote/RpcChannel.h"

namespace ProtoBufRemote {

class SocketRpcChannel : public RpcChannel
{
public:
	SocketRpcChannel(RpcController* controller, SOCKET socket);
	virtual ~SocketRpcChannel();

	void Start();

	void CloseAndJoin();

    bool IsClosed();

	virtual void Send(const RpcMessage& message);

	unsigned int GetAndClearBytesRead();

	unsigned int GetAndClearBytesWritten();

private:
    static unsigned int _stdcall ThreadRun(void* arg);
	void Run();

	SOCKET m_socket;
	HANDLE m_thread;
	WSAEVENT m_sendEvent;
	WSAEVENT m_terminateEvent;

	CRITICAL_SECTION m_sendMutex;

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

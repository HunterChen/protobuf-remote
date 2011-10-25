#ifndef PROTOBUFREMOTE_RPCCONTROLLER_H
#define PROTOBUFREMOTE_RPCCONTROLLER_H 1

#include "ProtoBufRemote/RpcMessage.h"


namespace ProtoBufRemote {

class RpcClient;
class RpcServer;
class RpcChannel;

class RpcController
{
public:
	RpcController();

	virtual void Send(const RpcMessage& message);

	virtual void Receive(const RpcMessage& message);

	void SetChannel(RpcChannel* channel) { m_channel = channel; }
	void SetClient(RpcClient* client) { m_client = client; }
	void SetServer(RpcServer* server) { m_server = server; }
	RpcChannel* GetChannel() const { return m_channel; }
	RpcClient* GetClient() const { return m_client; }
	RpcServer* GetServer() const { return m_server; }

private:
	RpcChannel* m_channel;
	RpcClient* m_client;
	RpcServer* m_server;
};

}

#endif

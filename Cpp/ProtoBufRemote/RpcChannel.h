#ifndef PROTOBUFREMOTE_RPCCHANNEL_H
#define PROTOBUFREMOTE_RPCCHANNEL_H 1

namespace ProtoBufRemote {

class RpcController;
class RpcMessage;

class RpcChannel
{
public:
	RpcChannel();

	RpcChannel(RpcController& controller);

	virtual void Send(const RpcMessage& message) = 0;

protected:
	RpcController* m_controller;
};

}

#endif

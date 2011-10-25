#ifndef PROTOBUFREMOTE_RPCCHANNEL_H
#define PROTOBUFREMOTE_RPCCHANNEL_H 1

#include "ProtoBufRemote/RpcMessage.h"


namespace ProtoBufRemote {

class RpcController;

class RpcChannel
{
public:
	RpcChannel(RpcController* controller);
	virtual ~RpcChannel() { }

	virtual void Send(const RpcMessage& message) = 0;

protected:
	RpcController* m_controller;
};

}

#endif

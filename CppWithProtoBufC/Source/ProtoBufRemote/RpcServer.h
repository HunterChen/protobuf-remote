#ifndef PROTOBUFREMOTE_RPCSERVER_H
#define PROTOBUFREMOTE_RPCSERVER_H 1

#include <unordered_map>
#include "ProtoBufRemote/RpcMessage.h"

namespace ProtoBufRemote {

class RpcController;
class RpcService;

class RpcServer
{
public:
	RpcServer(RpcController* controller);
    virtual ~RpcServer();

	bool RegisterService(RpcService* service);
	bool UnregisterService(RpcService* service);

	RpcService* FindService(const std::string& serviceName);

	virtual void ReceiveCall(const RpcMessage& message);

private:
	RpcController* m_controller;
	RpcMessage m_resultMessage;
    RpcMessageParameter m_resultParameterMessage;

	typedef std::unordered_map<std::string, RpcService*> ServiceMap;
	ServiceMap m_services;
};

}

#endif

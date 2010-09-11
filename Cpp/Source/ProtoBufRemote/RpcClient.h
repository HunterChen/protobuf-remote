#ifndef PROTOBUFREMOTE_RPCCLIENT_H
#define PROTOBUFREMOTE_RPCCLIENT_H 1

#include <string>
#include <unordered_map>

namespace ProtoBufRemote {

class ParameterList;
class PendingCall;
class RpcController;
class RpcMessage;

class RpcClient
{
public:
	RpcClient(RpcController* controller);

	virtual PendingCall* Call(const std::string& service, const std::string& method, const ParameterList& parameters);

	virtual void ReleaseCall(PendingCall* call);

	virtual void CallWithoutResult(const std::string& service, const std::string& method,
	                               const ParameterList& parameters);

	virtual void ReceiveResult(const RpcMessage& resultMessage);

private:
	RpcController* m_controller;
	int m_nextId;

	typedef std::unordered_map<int, PendingCall*> PendingCallMap;
	PendingCallMap m_pendingCalls;
};

}

#endif

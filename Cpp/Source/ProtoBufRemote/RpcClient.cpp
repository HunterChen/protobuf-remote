
#include "ProtoBufRemote/RpcClient.h"

#include "ProtoBufRemote/ParameterList.h"
#include "ProtoBufRemote/PendingCall.h"
#include "ProtoBufRemote/RpcController.h"


namespace ProtoBufRemote {

RpcClient::RpcClient(RpcController* controller)
	: m_controller(controller), m_nextId(0)
{
	if (m_controller)
		m_controller->SetClient(this);
}

PendingCall* RpcClient::Call(const std::string& service, const std::string& method, const ParameterList& parameters)
{
	assert(m_controller);

	RpcMessage* message = parameters.GetMessage();
	int id = m_nextId++;
	message->set_id(id);
	RpcMessage::Call* callMessage = message->mutable_call_message();
	callMessage->set_service(service);
	callMessage->set_method(method);
	callMessage->set_expects_result(true);

	PendingCall* call = new PendingCall(id);
	m_pendingCalls.insert(std::make_pair(id, call));

	m_controller->Send(*message);

	return call;
}

void RpcClient::ReleaseCall(PendingCall* call)
{
	m_pendingCalls.erase(call->GetId());
	delete call;
}

void RpcClient::CallWithoutResult(const std::string& service, const std::string& method,
                                  const ParameterList& parameters)
{
	assert(m_controller);

	RpcMessage* message = parameters.GetMessage();
	message->set_id(m_nextId++);
	RpcMessage::Call* callMessage = message->mutable_call_message();
	callMessage->set_service(service);
	callMessage->set_method(method);
	callMessage->set_expects_result(false);
	m_controller->Send(*message);
}

void RpcClient::ReceiveResult(const RpcMessage& resultMessage)
{
	PendingCallMap::iterator it = m_pendingCalls.find(resultMessage.id());
	if (it != m_pendingCalls.end())
	{
		PendingCall* call = it->second;
		const RpcMessage::Result& result = resultMessage.result_message();
		call->SetResult(result.has_call_result() ? &result.call_result() : NULL);
	}
}


}

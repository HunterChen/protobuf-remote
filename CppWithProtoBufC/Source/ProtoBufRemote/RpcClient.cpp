
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

PendingCall* RpcClient::Call(const char* service, const char* method, const ParameterList& parameters)
{
	assert(m_controller);

	RpcMessage* message = parameters.GetMessage();
	int id = m_nextId++;
	message->id = id;

    //service should actually already be set, just assert it
	RpcMessageCall* callMessage = message->call_message;
    assert(callMessage->service && strcmp(callMessage->service, service) == 0);

    //method should be preallocated, so just copy it
    assert(callMessage->method);
    assert(strlen(method) < 256);
    strcpy(callMessage->method, method);

	callMessage->expects_result = 1;

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

void RpcClient::CallWithoutResult(const char* service, const char* method, const ParameterList& parameters)
{
	assert(m_controller);

	RpcMessage* message = parameters.GetMessage();
	message->id = m_nextId++;

    //service should actually already be set, just assert it
	RpcMessageCall* callMessage = message->call_message;
    assert(callMessage->service && strcmp(callMessage->service, service) == 0);

    //method should be preallocated, so just copy it
    assert(callMessage->method);
    assert(strlen(method) < PBR_MAX_METHOD_LENGTH);
    strcpy(callMessage->method, method);

    callMessage->expects_result = 0;

	m_controller->Send(*message);
}

void RpcClient::ReceiveResult(const RpcMessage& resultMessage)
{
	PendingCallMap::iterator it = m_pendingCalls.find(resultMessage.id);
	if (it != m_pendingCalls.end())
	{
		PendingCall* call = it->second;
		const RpcMessageResult* result = resultMessage.result_message;
		call->SetResult(result->call_result);
	}
}


}

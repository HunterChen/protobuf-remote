
#include "ProtoBufRemote/RpcServer.h"

#include "ProtoBufRemote/ParameterList.h"
#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/RpcMessage.pb.h"
#include "ProtoBufRemote/RpcService.h"


namespace ProtoBufRemote {

RpcServer::RpcServer(RpcController* controller)
	: m_controller(controller)
{
	m_controller->SetServer(this);
}

bool RpcServer::RegisterService(RpcService* service)
{
	assert(service);
	return m_services.insert(std::make_pair(service->GetName(), service)).second;
}

bool RpcServer::UnregisterService(RpcService* service)
{
	assert(service);
	return (m_services.erase(service->GetName()) == 1);
}

RpcService* RpcServer::FindService(const std::string& serviceName)
{
	ServiceMap::iterator iter = m_services.find(serviceName);
	if (iter != m_services.end())
		return iter->second;
	return NULL;
}

void RpcServer::ReceiveCall(const RpcMessage& message)
{
	const RpcMessage::Call& callMessage = message.call_message();
	ServiceMap::iterator iter = m_services.find(callMessage.service());
	if (iter != m_services.end())
	{
		RpcService* service = iter->second;
		ParameterList parameters(message);

		RpcMessage::Result* result = m_resultMessage.mutable_result_message();
		MutableParameter resultParam(result->mutable_call_result());

		if (service->Call(callMessage.method().c_str(), parameters, &resultParam))
			result->clear_is_failed();
		else
			result->set_is_failed(true);

		if (callMessage.expects_result())
		{
			m_resultMessage.set_id(message.id());
			m_controller->Send(m_resultMessage);
		}
	}
}

}

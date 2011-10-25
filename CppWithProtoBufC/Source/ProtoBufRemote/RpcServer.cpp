
#include "ProtoBufRemote/RpcServer.h"

#include "ProtoBufRemote/ParameterList.h"
#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/RpcService.h"


namespace ProtoBufRemote {

RpcServer::RpcServer(RpcController* controller)
	: m_controller(controller)
{
	m_controller->SetServer(this);

    proto_buf_remote__rpc_message__init(&m_resultMessage);
    m_resultMessage.result_message = new RpcMessageResult;
    proto_buf_remote__rpc_message__result__init(m_resultMessage.result_message);
    m_resultMessage.result_message->call_result = &m_resultParameterMessage;
}

RpcServer::~RpcServer()
{
    delete m_resultMessage.result_message;
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
	const RpcMessageCall* callMessage = message.call_message;
	ServiceMap::iterator iter = m_services.find(callMessage->service);
	if (iter != m_services.end())
	{
		RpcService* service = iter->second;
		ParameterList parameters(message);

		RpcMessageResult* result = m_resultMessage.result_message;

        proto_buf_remote__rpc_message__parameter__init(&m_resultParameterMessage);

		MutableParameter resultParam(&m_resultParameterMessage);

		if (service->Call(callMessage->method, parameters, &resultParam))
        {
			result->has_is_failed = 0;
        }
		else
        {
            result->has_is_failed = 1;
			result->is_failed = 1;
        }

		if (callMessage->expects_result)
		{
			m_resultMessage.id = message.id;
			m_controller->Send(m_resultMessage);
		}

        FreeParameterMessageFields(&m_resultParameterMessage);
	}
}

}

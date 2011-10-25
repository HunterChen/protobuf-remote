
#include "ProtoBufRemote/Proxy.h"


namespace ProtoBufRemote
{

Proxy::Proxy(RpcClient* client, const char* serviceName)
	: m_client(client), m_parameters(&m_message)
{
    m_serviceName = new char[strlen(serviceName)+1];
    strcpy(m_serviceName, serviceName);

    proto_buf_remote__rpc_message__init(&m_message);
    m_message.call_message = new RpcMessageCall;
    proto_buf_remote__rpc_message__call__init(m_message.call_message);
    m_message.call_message->service = m_serviceName;
    m_message.call_message->method = new char[PBR_MAX_METHOD_LENGTH];
    m_message.call_message->parameters = new RpcMessageParameter*[PBR_MAX_PARAMETERS];
    for (int i=0; i<PBR_MAX_PARAMETERS; ++i)
    {
        m_message.call_message->parameters[i] = new RpcMessageParameter;
        proto_buf_remote__rpc_message__parameter__init(m_message.call_message->parameters[i]);
    }
}

Proxy::~Proxy()
{
    for (int i=0; i<PBR_MAX_PARAMETERS; ++i)
    {
        FreeParameterMessageFields(m_message.call_message->parameters[i]);
        delete m_message.call_message->parameters[i];
    }
    delete[] m_message.call_message->parameters;
    delete[] m_message.call_message->method;
    delete m_message.call_message;
    delete[] m_serviceName;
}

}

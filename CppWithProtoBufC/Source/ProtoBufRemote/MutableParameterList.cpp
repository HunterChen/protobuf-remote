
#include "ProtoBufRemote/MutableParameterList.h"


namespace ProtoBufRemote {

MutableParameterList::MutableParameterList(RpcMessage* message)
	: ParameterList(*message), m_message(message)
{
}

void MutableParameterList::Clear()
{
    for (unsigned int i=0; i<m_message->call_message->n_parameters; ++i)
    {
        FreeParameterMessageFields(m_message->call_message->parameters[i]);
    }
    m_message->call_message->n_parameters = 0;
}

MutableParameter MutableParameterList::Add()
{
    RpcMessageCall* callMessage = m_message->call_message;

    assert(callMessage->n_parameters < PBR_MAX_PARAMETERS);
    int paramIndex = callMessage->n_parameters;
    ++callMessage->n_parameters;

    proto_buf_remote__rpc_message__parameter__init(callMessage->parameters[paramIndex]);
	return MutableParameter(callMessage->parameters[paramIndex]);
}

}

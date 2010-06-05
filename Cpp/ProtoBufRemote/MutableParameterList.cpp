
#include "ProtoBufRemote/MutableParameterList.h"


namespace ProtoBufRemote {

MutableParameterList::MutableParameterList(RpcMessage* message)
	: ParameterList(*message), m_message(message)
{
}

void MutableParameterList::Clear()
{
	m_message->mutable_call_message()->clear_parameters();
}

MutableParameter MutableParameterList::Add()
{
	RpcMessage::Parameter* paramMessage = m_message->mutable_call_message()->add_parameters();
	return MutableParameter(paramMessage);
}

}

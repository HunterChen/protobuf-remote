
#include "ProtoBufRemote/ParameterList.h"


namespace ProtoBufRemote {

ParameterList::ParameterList(const RpcMessage& message)
	: m_message(message)
{
}

int ParameterList::GetNumParameters() const
{
	return m_message.call_message().parameters_size();
}

const Parameter ParameterList::GetParameter(int index) const
{
	return Parameter(m_message.call_message().parameters(index));
}

}

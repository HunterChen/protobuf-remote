
#include "ProtoBufRemote/MutableParameter.h"


namespace ProtoBufRemote {

MutableParameter::MutableParameter(RpcMessage::Parameter& message)
	: Parameter(message), m_message(message)
{
}

void MutableParameter::SetInt(int value)
{
	m_message.set_int_param(value);
}

void MutableParameter::SetBool(bool value)
{
	m_message.set_bool_param(value);
}

void MutableParameter::SetString(const std::string& value)
{
	m_message.set_string_param(value);
}

}

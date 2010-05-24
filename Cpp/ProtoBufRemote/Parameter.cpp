
#include "ProtoBufRemote/Parameter.h"

namespace ProtoBufRemote {

Parameter::Parameter(const RpcMessage::Parameter& message)
	: m_message(message)
{
}

bool Parameter::IsInt() const
{
	return m_message.has_int_param();
}

bool Parameter::IsBool() const
{
	return m_message.has_bool_param();
}

bool Parameter::IsString() const
{
	return m_message.has_string_param();
}

int Parameter::GetInt() const
{
	assert(m_message.has_int_param());
	return m_message.int_param();
}

bool Parameter::GetBool() const
{
	assert(m_message.has_bool_param());
	return m_message.bool_param();
}

const std::string& Parameter::GetString() const
{
	assert(m_message.has_string_param());
	return m_message.string_param();
}

}

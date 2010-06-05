
#include "ProtoBufRemote/MutableParameter.h"


namespace ProtoBufRemote {

MutableParameter::MutableParameter(RpcMessage::Parameter* message)
	: Parameter(*message), m_message(message)
{
}

void MutableParameter::SetChar(signed char value)
{
	m_message->set_int_param(value);
}

void MutableParameter::SetUnsignedChar(unsigned char value)
{
	m_message->set_uint_param(value);
}

void MutableParameter::SetShort(short value)
{
	m_message->set_int_param(value);
}

void MutableParameter::SetUnsignedShort(unsigned short value)
{
	m_message->set_uint_param(value);
}

void MutableParameter::SetInt(int value)
{
	m_message->set_int_param(value);
}

void MutableParameter::SetUnsignedInt(unsigned int value)
{
	m_message->set_uint_param(value);
}

void MutableParameter::SetInt64(long long value)
{
	m_message->set_int64_param(value);
}

void MutableParameter::SetUnsignedInt64(unsigned long long value)
{
	m_message->set_uint64_param(value);
}

void MutableParameter::SetBool(bool value)
{
	m_message->set_bool_param(value);
}

void MutableParameter::SetString(const std::string& value)
{
	m_message->set_string_param(value);
}

void MutableParameter::SetWChar(wchar_t value)
{
	m_message->set_uint_param(value);
}

}

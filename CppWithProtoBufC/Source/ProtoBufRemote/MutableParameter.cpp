
#include "ProtoBufRemote/MutableParameter.h"

#include <string.h>


namespace ProtoBufRemote {

MutableParameter::MutableParameter(RpcMessageParameter* message)
	: Parameter(*message), m_message(message)
{
}

void MutableParameter::SetChar(signed char value)
{
    m_message->has_int_param = 1;
	m_message->int_param = value;
}

void MutableParameter::SetUnsignedChar(unsigned char value)
{
    m_message->has_uint_param = 1;
	m_message->uint_param = value;
}

void MutableParameter::SetShort(short value)
{
    m_message->has_int_param = 1;
	m_message->int_param = value;
}

void MutableParameter::SetUnsignedShort(unsigned short value)
{
    m_message->has_uint_param = 1;
	m_message->uint_param = value;
}

void MutableParameter::SetInt(int value)
{
    m_message->has_int_param = 1;
	m_message->int_param = value;
}

void MutableParameter::SetUnsignedInt(unsigned int value)
{
    m_message->has_uint_param = 1;
	m_message->uint_param = value;
}

void MutableParameter::SetInt64(long long value)
{
    m_message->has_int64_param = 1;
	m_message->int64_param = value;
}

void MutableParameter::SetUnsignedInt64(unsigned long long value)
{
    m_message->has_uint64_param = 1;
	m_message->uint64_param = value;
}

void MutableParameter::SetFloat(float value)
{
    m_message->has_float_param = 1;
	m_message->float_param = value;
}

void MutableParameter::SetDouble(double value)
{
    m_message->has_double_param = 1;
	m_message->double_param = value;
}

void MutableParameter::SetBool(bool value)
{
    m_message->has_bool_param = 1;
	m_message->bool_param = value;
}

void MutableParameter::SetString(const char* value)
{
    assert(!m_message->string_param); //can only set once
    size_t len = strlen(value);
    m_message->string_param = new char[len+1];
    strcpy(m_message->string_param, value);
}

}

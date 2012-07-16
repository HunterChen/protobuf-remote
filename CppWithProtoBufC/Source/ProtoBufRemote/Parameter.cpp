
#include "ProtoBufRemote/Parameter.h"

namespace ProtoBufRemote {

Parameter::Parameter(const RpcMessageParameter& message)
	: m_message(message)
{
}

bool Parameter::IsChar() const
{
	return m_message.has_int_param ? true : false;
}

bool Parameter::IsUnsignedChar() const
{
	return m_message.has_uint_param ? true : false;
}

bool Parameter::IsShort() const
{
	return m_message.has_int_param ? true : false;
}

bool Parameter::IsUnsignedShort() const
{
	return m_message.has_uint_param ? true : false;
}

bool Parameter::IsInt() const
{
	return m_message.has_int_param ? true : false;
}

bool Parameter::IsUnsignedInt() const
{
	return m_message.has_uint_param ? true : false;
}

bool Parameter::IsInt64() const
{
	return m_message.has_int64_param ? true : false;
}

bool Parameter::IsUnsignedInt64() const
{
	return m_message.has_uint64_param ? true : false;
}

bool Parameter::IsBool() const
{
	return m_message.has_bool_param ? true : false;
}

bool Parameter::IsString() const
{
	return (m_message.string_param != NULL);
}

bool Parameter::IsProto() const
{
	return m_message.has_proto_param ? true : false;
}

signed char Parameter::GetChar() const
{
	assert(m_message.has_int_param);
	return static_cast<signed char>(m_message.int_param);
}

unsigned char Parameter::GetUnsignedChar() const
{
	assert(m_message.has_uint_param);
	return static_cast<unsigned char>(m_message.uint_param);
}

short Parameter::GetShort() const
{
	assert(m_message.has_int_param);
	return static_cast<short>(m_message.int_param);
}

unsigned short Parameter::GetUnsignedShort() const
{
	assert(m_message.has_uint_param);
	return static_cast<unsigned short>(m_message.uint_param);
}

int Parameter::GetInt() const
{
	assert(m_message.has_int_param);
	return m_message.int_param;
}

unsigned int Parameter::GetUnsignedInt() const
{
	assert(m_message.has_uint_param);
	return m_message.uint_param;
}

long long Parameter::GetInt64() const
{
	assert(m_message.has_int64_param);
	return m_message.int64_param;
}

unsigned long long Parameter::GetUnsignedInt64() const
{
	assert(m_message.has_uint64_param);
	return m_message.uint64_param;
}

bool Parameter::GetBool() const
{
	assert(m_message.has_bool_param);
	return m_message.bool_param ? true : false;
}

const char* Parameter::GetString() const
{
	assert(m_message.string_param);
	return m_message.string_param;
}

uint8_t* Parameter::GetProtoData() const
{
    assert(m_message.has_proto_param);
    return m_message.proto_param.data;
}

size_t Parameter::GetProtoDataSize() const
{
    assert(m_message.has_proto_param);
    return m_message.proto_param.len;
}

}

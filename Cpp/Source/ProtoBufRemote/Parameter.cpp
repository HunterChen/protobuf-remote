
#include "ProtoBufRemote/Parameter.h"

namespace ProtoBufRemote {

Parameter::Parameter(const RpcMessage::Parameter& message)
	: m_message(message)
{
}

bool Parameter::IsChar() const
{
	return m_message.has_int_param();
}

bool Parameter::IsUnsignedChar() const
{
	return m_message.has_uint_param();
}

bool Parameter::IsShort() const
{
	return m_message.has_int_param();
}

bool Parameter::IsUnsignedShort() const
{
	return m_message.has_uint_param();
}

bool Parameter::IsInt() const
{
	return m_message.has_int_param();
}

bool Parameter::IsUnsignedInt() const
{
	return m_message.has_uint_param();
}

bool Parameter::IsInt64() const
{
	return m_message.has_int64_param();
}

bool Parameter::IsUnsignedInt64() const
{
	return m_message.has_uint64_param();
}

bool Parameter::IsBool() const
{
	return m_message.has_bool_param();
}

bool Parameter::IsString() const
{
	return m_message.has_string_param();
}

bool Parameter::IsWChar() const
{
	return m_message.has_uint_param();
}

bool Parameter::IsProto() const
{
	return m_message.has_proto_param();
}

signed char Parameter::GetChar() const
{
	assert(m_message.has_int_param());
	return m_message.int_param();
}

unsigned char Parameter::GetUnsignedChar() const
{
	assert(m_message.has_uint_param());
	return m_message.uint_param();
}

short Parameter::GetShort() const
{
	assert(m_message.has_int_param());
	return m_message.int_param();
}

unsigned short Parameter::GetUnsignedShort() const
{
	assert(m_message.has_uint_param());
	return m_message.uint_param();
}

int Parameter::GetInt() const
{
	assert(m_message.has_int_param());
	return m_message.int_param();
}

unsigned int Parameter::GetUnsignedInt() const
{
	assert(m_message.has_uint_param());
	return m_message.uint_param();
}

long long Parameter::GetInt64() const
{
	assert(m_message.has_int64_param());
	return m_message.int64_param();
}

unsigned long long Parameter::GetUnsignedInt64() const
{
	assert(m_message.has_uint64_param());
	return m_message.uint64_param();
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

wchar_t Parameter::GetWChar() const
{
	assert(m_message.has_uint_param());
	return m_message.uint_param();
}

}

#ifndef PROTOBUFREMOTE_MUTABLEPARAMETER_H
#define PROTOBUFREMOTE_MUTABLEPARAMETER_H 1

#include "ProtoBufRemote/Parameter.h"

namespace ProtoBufRemote {

class MutableParameter : public Parameter
{
public:
	explicit MutableParameter(RpcMessage::Parameter* message);

	void SetChar(signed char value);
	void SetUnsignedChar(unsigned char value);
	void SetShort(short value);
	void SetUnsignedShort(unsigned short value);
	void SetInt(int value);
	void SetUnsignedInt(unsigned int value);
	void SetInt64(long long value);
	void SetUnsignedInt64(unsigned long long value);

	void SetBool(bool value);
	void SetString(const std::string& value);
	void SetWChar(wchar_t value);

	template<typename T>
	bool SetProto(const T& value)
	{
		return value.SerializeToString(m_message->mutable_proto_param());
	}

private:
	RpcMessage::Parameter* m_message;
};

}

#endif

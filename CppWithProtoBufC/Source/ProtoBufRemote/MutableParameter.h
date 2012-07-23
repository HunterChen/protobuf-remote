#ifndef PROTOBUFREMOTE_MUTABLEPARAMETER_H
#define PROTOBUFREMOTE_MUTABLEPARAMETER_H 1

#include "ProtoBufRemote/Parameter.h"

namespace ProtoBufRemote {

class MutableParameter : public Parameter
{
public:
	explicit MutableParameter(RpcMessageParameter* message);

	void SetChar(signed char value);
	void SetUnsignedChar(unsigned char value);
	void SetShort(short value);
	void SetUnsignedShort(unsigned short value);
	void SetInt(int value);
	void SetUnsignedInt(unsigned int value);
	void SetInt64(long long value);
	void SetUnsignedInt64(unsigned long long value);
    void SetFloat(float value);
    void SetDouble(double value);

	void SetBool(bool value);
	void SetString(const char* value);

	template<typename T>
	bool SetProto(const T& value)
	{
        assert(!m_message->proto_param.data && !m_message->has_proto_param); //can only set once
        m_message->has_proto_param = 1;
        m_message->proto_param.len = protobuf_c_message_get_packed_size((const ProtobufCMessage*)(&value));
        m_message->proto_param.data = new uint8_t[m_message->proto_param.len];
        size_t numWritten = protobuf_c_message_pack((const ProtobufCMessage*)(&value), m_message->proto_param.data);
		return (numWritten == m_message->proto_param.len);
	}

private:
	RpcMessageParameter* m_message;
};

}

#endif

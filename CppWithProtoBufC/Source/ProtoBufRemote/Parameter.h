#ifndef PROTOBUFREMOTE_PARAMETER_H
#define PROTOBUFREMOTE_PARAMETER_H 1

#include "ProtoBufRemote/RpcMessage.h"

namespace ProtoBufRemote {

class Parameter
{
public:
	explicit Parameter(const RpcMessageParameter& message);

	signed char GetChar() const;
	unsigned char GetUnsignedChar() const;
	short GetShort() const;
	unsigned short GetUnsignedShort() const;
	int GetInt() const;
	unsigned int GetUnsignedInt() const;
	long long GetInt64() const;
	unsigned long long GetUnsignedInt64() const;

    bool GetBool() const;
	const char* GetString() const;
	
    uint8_t* GetProtoData() const;
    size_t GetProtoDataSize() const;

	bool IsChar() const;
	bool IsUnsignedChar() const;
	bool IsShort() const;
	bool IsUnsignedShort() const;
	bool IsInt() const;
	bool IsUnsignedInt() const;
	bool IsInt64() const;
	bool IsUnsignedInt64() const;

	bool IsBool() const;
	bool IsString() const;
	
	bool IsProto() const;

private:
	const RpcMessageParameter& m_message;
};

}

#endif

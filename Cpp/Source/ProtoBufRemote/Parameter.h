#ifndef PROTOBUFREMOTE_PARAMETER_H
#define PROTOBUFREMOTE_PARAMETER_H 1

#include "ProtoBufRemote/RpcMessage.pb.h"

namespace ProtoBufRemote {

class Parameter
{
public:
	explicit Parameter(const RpcMessage::Parameter& message);

	signed char GetChar() const;
	unsigned char GetUnsignedChar() const;
	short GetShort() const;
	unsigned short GetUnsignedShort() const;
	int GetInt() const;
	unsigned int GetUnsignedInt() const;
	long long GetInt64() const;
	unsigned long long GetUnsignedInt64() const;

	bool GetBool() const;
	const std::string& GetString() const;
	void GetString(std::string* str) const;
	wchar_t GetWChar() const;
	
	template<typename T>
	void GetProto(T* paramMessage) const
	{
		paramMessage->ParseFromString(m_message.proto_param());
	}

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
	bool IsWChar() const;
	
	bool IsProto() const;

private:
	const RpcMessage::Parameter& m_message;
};

}

#endif

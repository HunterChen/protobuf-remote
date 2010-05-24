#ifndef PROTOBUFREMOTE_PARAMETER_H
#define PROTOBUFREMOTE_PARAMETER_H 1

#include "ProtoBufRemote/RpcMessage.pb.h"

namespace ProtoBufRemote {

class Parameter
{
public:
	explicit Parameter(const RpcMessage::Parameter& message);

	int GetInt() const;
	bool GetBool() const;
	const std::string& GetString() const;

	bool IsInt() const;
	bool IsBool() const;
	bool IsString() const;

private:
	const RpcMessage::Parameter& m_message;
};

}

#endif

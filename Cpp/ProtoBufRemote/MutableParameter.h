#ifndef PROTOBUFREMOTE_MUTABLEPARAMETER_H
#define PROTOBUFREMOTE_MUTABLEPARAMETER_H 1

#include "ProtoBufRemote/Parameter.h"

namespace ProtoBufRemote {

class MutableParameter : public Parameter
{
public:
	explicit MutableParameter(RpcMessage::Parameter& message);

	void SetInt(int value);
	void SetBool(bool value);
	void SetString(const std::string& value);

private:
	RpcMessage::Parameter& m_message;
};

}

#endif

#ifndef PROTOBUFREMOTE_PARAMETERLIST_H
#define PROTOBUFREMOTE_PARAMETERLIST_H 1

#include "ProtoBufRemote/MutableParameter.h"
#include "ProtoBufRemote/RpcMessage.h"

namespace ProtoBufRemote {

class RpcClient;

class ParameterList
{
public:
	ParameterList(const RpcMessage& message);

	int GetNumParameters() const;

	const Parameter GetParameter(int index) const;

private:
	friend RpcClient;

	RpcMessage* GetMessage() const { return const_cast<RpcMessage*>(&m_message); }

	const RpcMessage& m_message;
};

}

#endif

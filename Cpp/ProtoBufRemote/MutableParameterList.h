#ifndef PROTOBUFREMOTE_MUTABLEPARAMETERLIST_H
#define PROTOBUFREMOTE_MUTABLEPARAMETERLIST_H 1

#include "ProtoBufRemote/ParameterList.h"

namespace ProtoBufRemote {

class MutableParameterList : public ParameterList
{
public:
	MutableParameterList(RpcMessage& message);

	void Clear();

	MutableParameter Add();

private:
	RpcMessage& m_message;
};

}

#endif

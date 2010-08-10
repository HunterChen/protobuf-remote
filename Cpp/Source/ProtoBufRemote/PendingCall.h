#ifndef PROTOBUFREMOTE_PENDINGCALL_H
#define PROTOBUFREMOTE_PENDINGCALL_H 1

#include "ProtoBufRemote/RpcMessage.pb.h"

namespace ProtoBufRemote {

class Parameter;
class RpcClient;

class PendingCall
{
public:
	PendingCall(int id);

	int GetId() const { return m_id; }

	void Wait();

	const Parameter* GetResult();

	void SetResult(const RpcMessage::Parameter* parameter);

private:
	int m_id;
	bool m_isCompleted;
	Parameter* m_result;
};

}

#endif

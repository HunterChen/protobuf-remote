#ifndef PROTOBUFREMOTE_PENDINGCALL_H
#define PROTOBUFREMOTE_PENDINGCALL_H 1

#include "ProtoBufRemote/RpcMessage.h"

typedef void *HANDLE; //don't want windows.h included by a header

namespace ProtoBufRemote {

class Parameter;
class RpcClient;

class PendingCall
{
public:
	PendingCall(int id);

    ~PendingCall();

	int GetId() const { return m_id; }

	void Wait();

	const Parameter* GetResult();

	void SetResult(const RpcMessageParameter* parameter);

private:
	int m_id;
    HANDLE m_event;
	bool m_isCompleted;
    RpcMessageParameter* m_resultMessage;
	Parameter* m_result;
};

}

#endif

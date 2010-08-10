
#include "ProtoBufRemote/PendingCall.h"

#include "ProtoBufRemote/Parameter.h"


namespace ProtoBufRemote {

PendingCall::PendingCall(int id)
	: m_id(id), m_isCompleted(false), m_result(NULL)
{
}

void PendingCall::Wait()
{
}

const Parameter* PendingCall::GetResult()
{
	return m_result;
}

void PendingCall::SetResult(const RpcMessage::Parameter* parameter)
{
	if (parameter)
		m_result = new Parameter(*parameter);
	m_isCompleted = true;
}

}

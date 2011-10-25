
#include "ProtoBufRemote/PendingCall.h"

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

#include "ProtoBufRemote/Parameter.h"


namespace ProtoBufRemote {

PendingCall::PendingCall(int id)
	: m_id(id), m_isCompleted(false), m_resultMessage(NULL), m_result(NULL)
{
    m_event = CreateEvent(NULL, TRUE, FALSE, NULL);
}

PendingCall::~PendingCall()
{
    CloseHandle(m_event);
    delete m_result;
    if (m_resultMessage)
    {
        FreeParameterMessageFields(m_resultMessage);
        delete m_resultMessage;
    }
}

void PendingCall::Wait()
{
    WaitForSingleObject(m_event, INFINITE);
}

const Parameter* PendingCall::GetResult()
{
	return m_result;
}

void PendingCall::SetResult(const RpcMessageParameter* parameter)
{
	if (parameter)
    {
        m_resultMessage = new RpcMessageParameter;
        CopyParameterMessage(*parameter, m_resultMessage);

		m_result = new Parameter(*m_resultMessage);
    }
	m_isCompleted = true;
    SetEvent(m_event);
}

}

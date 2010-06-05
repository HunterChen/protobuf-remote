#ifndef PROTOBUFREMOTETEST_SAMPLESERVICEPROXY_H
#define PROTOBUFREMOTETEST_SAMPLESERVICEPROXY_H 1

#include "ProtoBufRemote/PendingCall.h"
#include "ProtoBufRemote/Proxy.h"

class SampleServiceProxy : public ProtoBufRemote::Proxy
{
public:
	SampleServiceProxy(ProtoBufRemote::RpcClient* client)
		: ProtoBufRemote::Proxy(client, "SampleService")
	{
	}

	int GetSquare(int x)
	{
		m_parameters.Clear();
		m_parameters.Add().SetInt(x);
		ProtoBufRemote::PendingCall* call = m_client->Call(m_serviceName, "GetSquare", m_parameters);
		call->Wait();
		int result = call->GetResult()->GetInt();
		m_client->ReleaseCall(call);
		return result;
	}

	void DoStuff(const std::string& str)
	{
		m_parameters.Clear();
		m_parameters.Add().SetString(str);
		m_client->CallWithoutResult(m_serviceName, "DoStuff", m_parameters);
	}
};

#endif

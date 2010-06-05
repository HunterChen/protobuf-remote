#ifndef PROTOBUFREMOTE_PROXY_H
#define PROTOBUFREMOTE_PROXY_H 1

#include <string>
#include "ProtoBufRemote/MutableParameterList.h"

namespace ProtoBufRemote {

class RpcClient;

class Proxy
{
public:
	Proxy(RpcClient* client, const char* serviceName)
		: m_client(client), m_serviceName(serviceName), m_parameters(&m_message)
	{
	}

protected:
	RpcClient* m_client;
	std::string m_serviceName;
	RpcMessage m_message;
	MutableParameterList m_parameters;
};

}

#endif

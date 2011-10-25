#ifndef PROTOBUFREMOTE_PROXY_H
#define PROTOBUFREMOTE_PROXY_H 1

#include <string>
#include "ProtoBufRemote/MutableParameterList.h"

namespace ProtoBufRemote {

class RpcClient;

class Proxy
{
public:
	Proxy(RpcClient* client, const char* serviceName);

    virtual ~Proxy();

protected:
	RpcClient* m_client;
	char* m_serviceName;
	RpcMessage m_message;
	MutableParameterList m_parameters;
};

}

#endif

#ifndef PROTOBUFREMOTE_RPCSERVICE_H
#define PROTOBUFREMOTE_RPCSERVICE_H 1

#include <string>

namespace ProtoBufRemote
{

class ParameterList;
class MutableParameter;

class RpcService
{
public:
	RpcService(const char* serviceName) : m_serviceName(serviceName) { }

	const std::string& GetName() const { return m_serviceName; }

	virtual bool Call(const char* methodName, const ParameterList& parameters, MutableParameter* result) = 0;

protected:
	std::string m_serviceName;
};

}

#endif

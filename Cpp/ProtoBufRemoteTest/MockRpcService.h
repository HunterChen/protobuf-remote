#ifndef PROTOBUFREMOTETEST_MOCKRPCSERVICE_H
#define PROTOBUFREMOTETEST_MOCKRPCSERVICE_H 1

#include <gmock/gmock.h>
#include "ProtoBufRemote/MutableParameter.h"
#include "ProtoBufRemote/ParameterList.h"
#include "ProtoBufRemote/RpcService.h"

class MockRpcService : public ProtoBufRemote::RpcService
{
public:
	MockRpcService(const char* serviceName) : ProtoBufRemote::RpcService(serviceName) { }

	MOCK_METHOD3(Call, bool(const char*, const ProtoBufRemote::ParameterList&, ProtoBufRemote::MutableParameter*));
};

#endif

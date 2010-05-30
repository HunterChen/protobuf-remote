#ifndef PROTOBUFREMOTETEST_MOCKRPCSERVER_H
#define PROTOBUFREMOTETEST_MOCKRPCSERVER_H 1

#include <gmock/gmock.h>
#include "ProtoBufRemote/RpcServer.h"
#include "ProtoBufRemote/RpcMessage.pb.h"

class MockRpcServer : public ProtoBufRemote::RpcServer
{
public:
	MockRpcServer(ProtoBufRemote::RpcController* controller) : ProtoBufRemote::RpcServer(controller) { }

	MOCK_METHOD1(ReceiveCall, void(const ProtoBufRemote::RpcMessage&));
};

#endif

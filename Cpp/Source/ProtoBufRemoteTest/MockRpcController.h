#ifndef PROTOBUFREMOTETEST_MOCKRPCCONTROLLER_H
#define PROTOBUFREMOTETEST_MOCKRPCCONTROLLER_H 1

#include <gmock/gmock.h>
#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/RpcMessage.pb.h"

class MockRpcController : public ProtoBufRemote::RpcController
{
public:
	MOCK_METHOD1(Send, void(const ProtoBufRemote::RpcMessage&));
};

#endif

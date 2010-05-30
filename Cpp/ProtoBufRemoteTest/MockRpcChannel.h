#ifndef PROTOBUFREMOTETEST_MOCKRPCCHANNEL_H
#define PROTOBUFREMOTETEST_MOCKRPCCHANNEL_H 1

#include <gmock/gmock.h>
#include "ProtoBufRemote/RpcChannel.h"
#include "ProtoBufRemote/RpcMessage.pb.h"

class MockRpcChannel : public ProtoBufRemote::RpcChannel
{
public:
	MOCK_METHOD1(Send, void(const ProtoBufRemote::RpcMessage&));
};

#endif

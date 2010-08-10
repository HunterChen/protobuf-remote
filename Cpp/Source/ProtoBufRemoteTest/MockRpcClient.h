#ifndef PROTOBUFREMOTETEST_MOCKRPCCLIENT_H
#define PROTOBUFREMOTETEST_MOCKRPCCLIENT_H 1

#include <gmock/gmock.h>
#include "ProtoBufRemote/ParameterList.h"
#include "ProtoBufRemote/RpcClient.h"
#include "ProtoBufRemote/RpcMessage.pb.h"

class MockRpcClient : public ProtoBufRemote::RpcClient
{
public:
	MockRpcClient() : ProtoBufRemote::RpcClient(NULL) { }

	MOCK_METHOD1(ReceiveResult, void(const ProtoBufRemote::RpcMessage&));
	MOCK_METHOD3(Call, ProtoBufRemote::PendingCall*(const std::string&, const std::string&,
	                                                const ProtoBufRemote::ParameterList&));
	MOCK_METHOD1(ReleaseCall, void(ProtoBufRemote::PendingCall*));
	MOCK_METHOD3(CallWithoutResult, void(const std::string&, const std::string&,
		                                 const ProtoBufRemote::ParameterList&));
};

#endif


#include <gtest/gtest.h>
#include <gmock/gmock.h>

#include "ProtoBufRemote/RpcController.h"

#include "ProtoBufRemoteTest/MockRpcChannel.h"
#include "ProtoBufRemoteTest/MockRpcClient.h"
#include "ProtoBufRemoteTest/MockRpcServer.h"


using ProtoBufRemote::RpcController;
using ProtoBufRemote::RpcMessage;
using ::testing::Ref;


TEST(RpcControllerTest, Send)
{
	MockRpcChannel channel;
	RpcController controller;
	controller.SetChannel(&channel);
	RpcMessage testMessage;

	EXPECT_CALL(channel, Send(Ref(testMessage)));
	controller.Send(testMessage);
}

TEST(RpcControllerTest, ReceiveCall)
{
	RpcController controller;
	MockRpcServer server(&controller);
	controller.SetServer(&server);
	RpcMessage testMessage;
	testMessage.mutable_call_message();

	EXPECT_CALL(server, ReceiveCall(Ref(testMessage)));
	controller.Receive(testMessage);
}

TEST(RpcControllerTest, ReceiveResult)
{
	MockRpcClient client;
	RpcController controller;
	controller.SetClient(&client);
	RpcMessage testMessage;
	testMessage.mutable_result_message();

	EXPECT_CALL(client, ReceiveResult(Ref(testMessage)));
	controller.Receive(testMessage);
}

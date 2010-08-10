
#include <gtest/gtest.h>

#include "ProtoBufRemote/PendingCall.h"
#include "ProtoBufRemoteTest/MockRpcClient.h"
#include "ProtoBufRemoteTest/SampleServiceProxy.h"


using ProtoBufRemote::ParameterList;
using ProtoBufRemote::PendingCall;
using ProtoBufRemote::RpcMessage;
using ::testing::Truly;
using ::testing::Return;

namespace {

bool IsGetSquareParameterListCorrect(const ParameterList& paramList)
{
	return (paramList.GetNumParameters() == 1 && paramList.GetParameter(0).GetInt() == 5);
}
bool IsDoStuffParameterListCorrect(const ParameterList& paramList)
{
	return (paramList.GetNumParameters() == 1 && paramList.GetParameter(0).GetString() == "hello");
}

}

TEST(ProxyTest, Call)
{
	MockRpcClient client;
	SampleServiceProxy proxy(&client);

	PendingCall call(0);
	RpcMessage::Parameter resultParam;
	resultParam.set_int_param(25);
	call.SetResult(&resultParam);
	EXPECT_CALL(client, Call("SampleService", "GetSquare", Truly(IsGetSquareParameterListCorrect)))
		.WillOnce(Return(&call));
	EXPECT_CALL(client, ReleaseCall(&call));

	int result = proxy.GetSquare(5);

	EXPECT_EQ(25, result);
}

TEST(ProxyTest, CallWithoutResult)
{
	MockRpcClient client;
	SampleServiceProxy proxy(&client);

	EXPECT_CALL(client, CallWithoutResult("SampleService", "DoStuff", Truly(IsDoStuffParameterListCorrect)));

	proxy.DoStuff("hello");
}

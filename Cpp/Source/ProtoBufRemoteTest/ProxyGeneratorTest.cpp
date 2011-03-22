
#include <gtest/gtest.h>

#include "ProtoBufRemote/Generators.h"
#include "ProtoBufRemoteTest/MockRpcClient.h"

using ProtoBufRemote::ParameterList;
using ProtoBufRemote::PendingCall;
using ProtoBufRemote::RpcMessage;
using ::testing::Return;
using ::testing::Truly;

PBR_SERVICE(SampleService2,
	PBR_METHOD(GetSquare, PBR_INT(PBR_INT))
	PBR_METHOD(DoStuff, PBR_VOID(PBR_STRING))
	PBR_METHOD(DoOtherStuff, PBR_VOID(PBR_STRING PBR_INT PBR_BOOL))
	PBR_METHOD(DoEvenMoreStuff, PBR_VOID(PBR_VOID))
    PBR_METHOD(GetString, PBR_STRING(PBR_VOID))
    PBR_METHOD(GetString2, PBR_STRING(PBR_INT))
)

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

TEST(ProxyGeneratorTest, Call)
{
	MockRpcClient client;
	SampleService2Proxy proxy(&client);

	PendingCall call(0);
	RpcMessage::Parameter resultParam;
	resultParam.set_int_param(25);
	call.SetResult(&resultParam);
	EXPECT_CALL(client, Call("SampleService2", "GetSquare", Truly(IsGetSquareParameterListCorrect)))
		.WillOnce(Return(&call));
	EXPECT_CALL(client, ReleaseCall(&call));

	int result = proxy.GetSquare(5);

	EXPECT_EQ(25, result);
}

TEST(ProxyGeneratorTest, CallWithoutResult)
{
	MockRpcClient client;
	SampleService2Proxy proxy(&client);

	EXPECT_CALL(client, CallWithoutResult("SampleService2", "DoStuff", Truly(IsDoStuffParameterListCorrect)));

	proxy.DoStuff("hello");
}

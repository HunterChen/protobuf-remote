
#include <gtest/gtest.h>
#include <gmock/gmock.h>

#include "ProtoBufRemote/Generators.h"
#include "ProtoBufRemote/MutableParameterList.h"
#include "ProtoBufRemoteTest/MockRpcServer.h"

using ProtoBufRemote::MutableParameter;
using ProtoBufRemote::MutableParameterList;
using ProtoBufRemote::ParameterList;
using ProtoBufRemote::PendingCall;
using ProtoBufRemote::RpcMessage;
using ::testing::Return;
using ::testing::Truly;

PBR_SERVICE(SampleService2,
	PBR_METHOD(GetSquare, PBR_INT(PBR_INT))
	PBR_METHOD(GetString, PBR_STRING(PBR_INT))
	PBR_METHOD(DoStuff, PBR_VOID(PBR_STRING))
	PBR_METHOD(DoOtherStuff, PBR_VOID(PBR_STRING PBR_INT PBR_BOOL))
	PBR_METHOD(DoEvenMoreStuff, PBR_VOID(PBR_VOID))
)

class MockSampleService2 : public SampleService2Stub
{
public:
	MOCK_METHOD1(GetSquare, int(int));
	MOCK_METHOD2(GetString, void(int, std::string*));
	MOCK_METHOD1(DoStuff, void(const std::string&));
	MOCK_METHOD3(DoOtherStuff, void(const std::string&, int, bool));
	MOCK_METHOD0(DoEvenMoreStuff, void(void));
};

TEST(StubGeneratorTest, Call)
{
	MockSampleService2 service;

	EXPECT_CALL(service, GetSquare(5)).WillOnce(Return(25));

	RpcMessage message;
	MutableParameterList parameters(message);
	parameters.Add().SetInt(5);
	RpcMessage::Parameter resultMessage;
	MutableParameter result(&resultMessage);

	bool isSuccess = service.Call("GetSquare", parameters, &result);

	EXPECT_TRUE(isSuccess);
	EXPECT_EQ(25, result.GetInt());
}

TEST(StubGeneratorTest, CallWithoutResult)
{
	MockSampleService2 service;

	EXPECT_CALL(service, DoStuff("hello"));

	RpcMessage message;
	MutableParameterList parameters(message);
	parameters.Add().SetString("hello");

	bool isSuccess = service.Call("DoStuff", parameters, NULL);

	EXPECT_TRUE(isSuccess);
}

TEST(StubGeneratorTest, CallUnknownMethod)
{
	MockSampleService2 service;

	RpcMessage message;
	MutableParameterList parameters(message);

	bool isSuccess = service.Call("NotAMethod", parameters, NULL);

	EXPECT_FALSE(isSuccess);
}

TEST(StubGeneratorTest, CallWrongParameterCount)
{
	MockSampleService2 service;

	RpcMessage message;
	MutableParameterList parameters(message);
	parameters.Add().SetInt(5);
	parameters.Add().SetInt(6);
	RpcMessage::Parameter resultMessage;
	MutableParameter result(&resultMessage);

	bool isSuccess = service.Call("GetSquare", parameters, &result);

	EXPECT_FALSE(isSuccess);
}

TEST(StubGeneratorTest, CallWrongParameterType)
{
	MockSampleService2 service;

	RpcMessage message;
	MutableParameterList parameters(message);
	parameters.Add().SetString("hello");
	RpcMessage::Parameter resultMessage;
	MutableParameter result(&resultMessage);

	bool isSuccess = service.Call("GetSquare", parameters, &result);

	EXPECT_FALSE(isSuccess);
}

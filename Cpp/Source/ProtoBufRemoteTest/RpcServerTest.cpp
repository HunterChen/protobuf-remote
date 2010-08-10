
#include <gtest/gtest.h>

#include "ProtoBufRemote/RpcServer.h"
#include "ProtoBufRemote/RpcService.h"
#include "ProtoBufRemoteTest/MockRpcController.h"
#include "ProtoBufRemoteTest/MockRpcService.h"


using ProtoBufRemote::MutableParameter;
using ProtoBufRemote::ParameterList;
using ProtoBufRemote::RpcMessage;
using ProtoBufRemote::RpcServer;
using ProtoBufRemote::RpcService;
using ::testing::_;
using ::testing::StrEq;
using ::testing::Truly;
using ::testing::Invoke;
using ::testing::Unused;
using ::testing::Return;


class RpcServerTest : public ::testing::Test
{
public:
	RpcServerTest()
		: m_server(&m_controller), m_service("TestService")
	{
	}

	static bool CallTestMethod(Unused, Unused, MutableParameter* result)
	{
		result->SetInt(44);
		return true;
	}

	static bool IsParameterListCorrect(const ParameterList& parameters)
	{
		return (parameters.GetNumParameters() == 1 && parameters.GetParameter(0).GetInt() == 43);
	}

	static bool IsResultMessageCorrect(const RpcMessage& message)
	{
		if (!message.has_id() || !message.has_result_message())
			return false;
		if (message.id() != 42)
			return false;
		const RpcMessage::Result& resultMessage = message.result_message();
		if (resultMessage.is_failed() || !resultMessage.has_call_result())
			return false;
		const RpcMessage::Parameter& result = resultMessage.call_result();
		if (!result.has_int_param() || result.int_param() != 44)
			return false;
		return true;
	}

	static bool IsResultErrorMessageCorrect(const RpcMessage& message)
	{
		if (!message.has_id() || !message.has_result_message())
			return false;
		if (message.id() != 42)
			return false;
		const RpcMessage::Result& resultMessage = message.result_message();
		if (!resultMessage.is_failed())
			return false;
		return true;
	}

protected:
	MockRpcController m_controller;
	RpcServer m_server;
	MockRpcService m_service;
};

TEST_F(RpcServerTest, Register)
{
	bool isSuccess = m_server.RegisterService(&m_service);
	EXPECT_TRUE(isSuccess);
	isSuccess = m_server.RegisterService(&m_service);
	EXPECT_FALSE(isSuccess);
	isSuccess = m_server.UnregisterService(&m_service);
	EXPECT_TRUE(isSuccess);
	isSuccess = m_server.UnregisterService(&m_service);
	EXPECT_FALSE(isSuccess);
}

TEST_F(RpcServerTest, ReceiveCall)
{
	m_server.RegisterService(&m_service);

	RpcMessage message;
	message.set_id(42);
	RpcMessage::Call* callMessage = message.mutable_call_message();
	callMessage->set_service("TestService");
	callMessage->set_method("TestMethod");
	callMessage->set_expects_result(true);
	RpcMessage::Parameter* param = callMessage->add_parameters();
	param->set_int_param(43);

	EXPECT_CALL(m_service, Call(StrEq("TestMethod"), Truly(IsParameterListCorrect), _))
		.WillOnce(Invoke(CallTestMethod));
	EXPECT_CALL(m_controller, Send(Truly(IsResultMessageCorrect)));

	m_server.ReceiveCall(message);
}

TEST_F(RpcServerTest, ReceiveCallWithoutResult)
{
	m_server.RegisterService(&m_service);

	RpcMessage message;
	message.set_id(42);
	RpcMessage::Call* callMessage = message.mutable_call_message();
	callMessage->set_service("TestService");
	callMessage->set_method("TestMethod");
	callMessage->set_expects_result(false);
	RpcMessage::Parameter* param = callMessage->add_parameters();
	param->set_int_param(43);

	EXPECT_CALL(m_service, Call(StrEq("TestMethod"), Truly(IsParameterListCorrect), _))
		.WillOnce(Invoke(&RpcServerTest::CallTestMethod));
	EXPECT_CALL(m_controller, Send(_)).Times(0);

	m_server.ReceiveCall(message);
}

TEST_F(RpcServerTest, ReceiveCallError)
{
	m_server.RegisterService(&m_service);

	RpcMessage message;
	message.set_id(42);
	RpcMessage::Call* callMessage = message.mutable_call_message();
	callMessage->set_service("TestService");
	callMessage->set_method("TestMethod");
	callMessage->set_expects_result(true);
	RpcMessage::Parameter* param = callMessage->add_parameters();
	param->set_int_param(43);

	EXPECT_CALL(m_service, Call(StrEq("TestMethod"), Truly(IsParameterListCorrect), _)).WillOnce(Return(false));
	EXPECT_CALL(m_controller, Send(Truly(IsResultErrorMessageCorrect)));

	m_server.ReceiveCall(message);
}

TEST_F(RpcServerTest, ReceiveCallErrorWithoutResult)
{
	m_server.RegisterService(&m_service);

	RpcMessage message;
	message.set_id(42);
	RpcMessage::Call* callMessage = message.mutable_call_message();
	callMessage->set_service("TestService");
	callMessage->set_method("TestMethod");
	callMessage->set_expects_result(false);
	RpcMessage::Parameter* param = callMessage->add_parameters();
	param->set_int_param(43);

	EXPECT_CALL(m_service, Call(StrEq("TestMethod"), Truly(IsParameterListCorrect), _)).WillOnce(Return(false));
	EXPECT_CALL(m_controller, Send(_)).Times(0);

	m_server.ReceiveCall(message);
}

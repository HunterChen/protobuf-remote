
#include <gtest/gtest.h>

#include "ProtoBufRemote/MutableParameterList.h"
#include "ProtoBufRemote/PendingCall.h"
#include "ProtoBufRemote/RpcClient.h"
#include "ProtoBufRemoteTest/MockRpcController.h"

using ProtoBufRemote::RpcClient;
using ProtoBufRemote::RpcMessage;
using ProtoBufRemote::MutableParameterList;
using ProtoBufRemote::PendingCall;
using ::testing::_;
using ::testing::AllOf;
using ::testing::Property;
using ::testing::Eq;
using ::testing::StrEq;
using ::testing::Truly;
using ::testing::Return;

class RpcClientTest : public ::testing::Test
{
protected:
	virtual void SetUp()
	{
		m_client = new RpcClient(&m_controller);
	}

	virtual void TearDown()
	{
		delete m_client;
	}

public:
	void SendResult(const RpcMessage& callMessage)
	{
		m_resultMessage.set_id(callMessage.id());
		RpcMessage::Result* result = m_resultMessage.mutable_result_message();
		RpcMessage::Parameter* param = result->mutable_call_result();
		param->set_int_param(43);

		m_client->ReceiveResult(m_resultMessage);
	}

protected:
	static bool IsCallMessageCorrect(const RpcMessage& message)
	{
		if (!message.has_id() || !message.has_call_message())
			return false;
		if (message.call_message().service() != "ServiceName" || message.call_message().method() != "MethodName")
			return false;
		if (message.call_message().parameters_size() != 1)
			return false;
		const RpcMessage::Parameter& parameter = message.call_message().parameters(0);
		if (!parameter.has_int_param() || parameter.int_param() != 42)
			return false;
		return true;
	}

	MockRpcController m_controller;
	RpcClient* m_client;
	RpcMessage m_resultMessage;
};

TEST_F(RpcClientTest, CallWithoutResult)
{
	EXPECT_CALL(m_controller, Send(AllOf(Truly(IsCallMessageCorrect),
		Property(&RpcMessage::call_message, Property(&RpcMessage::Call::expects_result, Eq(false))))));

	RpcMessage message;
	MutableParameterList parameters(&message);
	parameters.Add().SetInt(42);
	m_client->CallWithoutResult("ServiceName", "MethodName", parameters);
}

TEST_F(RpcClientTest, Call)
{
	EXPECT_CALL(m_controller, Send(AllOf(Truly(IsCallMessageCorrect),
		Property(&RpcMessage::call_message, Property(&RpcMessage::Call::expects_result, Eq(true))))))
		.WillOnce(Invoke(this, &RpcClientTest::SendResult));

	RpcMessage message;
	MutableParameterList parameters(&message);
	parameters.Add().SetInt(42);
	PendingCall* call = m_client->Call("ServiceName", "MethodName", parameters);
	call->Wait();
	EXPECT_EQ(43, call->GetResult()->GetInt());
	m_client->ReleaseCall(call);
}

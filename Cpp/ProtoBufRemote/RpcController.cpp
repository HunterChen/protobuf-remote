
#include "ProtoBufRemote/RpcController.h"

#include <cstdlib>
#include <cassert>

#include "ProtoBufRemote/RpcChannel.h"
#include "ProtoBufRemote/RpcClient.h"
#include "ProtoBufRemote/RpcMessage.pb.h"
#include "ProtoBufRemote/RpcServer.h"

namespace ProtoBufRemote {

RpcController::RpcController()
	: m_channel(NULL), m_client(NULL), m_server(NULL)
{
}

void RpcController::Send(const RpcMessage& message)
{
	assert(m_channel);
	m_channel->Send(message);
}

void RpcController::Receive(const RpcMessage& message)
{
	if (message.has_call_message())
	{
		assert(m_server);
		m_server->ReceiveCall(message);
	}
	else
	{
		assert(message.has_result_message());
		assert(m_client);
		m_client->ReceiveResult(message);
	}
}

}

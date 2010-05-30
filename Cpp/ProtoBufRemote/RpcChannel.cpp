
#include "ProtoBufRemote/RpcChannel.h"

#include <cstdlib>

#include "ProtoBufRemote/RpcController.h"

namespace ProtoBufRemote {

RpcChannel::RpcChannel()
	: m_controller(NULL)
{
}

RpcChannel::RpcChannel(RpcController& controller)
	: m_controller(&controller)
{
	m_controller->SetChannel(this);
}

}


#include "ProtoBufRemote/RpcChannel.h"

#include <cstdlib>

#include "ProtoBufRemote/RpcController.h"

namespace ProtoBufRemote {

RpcChannel::RpcChannel(RpcController* controller)
	: m_controller(controller)
{
	if (m_controller)
		m_controller->SetChannel(this);
}

}

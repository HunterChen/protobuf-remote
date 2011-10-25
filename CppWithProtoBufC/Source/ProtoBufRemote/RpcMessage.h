#ifndef PROTOBUFREMOTE_RPCMESSAGE_H_
#define PROTOBUFREMOTE_RPCMESSAGE_H_ 1

#include "ProtoBufRemote/RpcMessage.pb-c.h"

namespace ProtoBufRemote
{

typedef ProtoBufRemote__RpcMessage RpcMessage;
typedef ProtoBufRemote__RpcMessage__Parameter RpcMessageParameter;
typedef ProtoBufRemote__RpcMessage__Call RpcMessageCall;
typedef ProtoBufRemote__RpcMessage__Result RpcMessageResult;

void CopyParameterMessage(const RpcMessageParameter& source, RpcMessageParameter* dest);
void FreeParameterMessageFields(RpcMessageParameter* paramMessage);

static const int PBR_MAX_PARAMETERS = 16;
static const int PBR_MAX_METHOD_LENGTH = 256;

}

#endif


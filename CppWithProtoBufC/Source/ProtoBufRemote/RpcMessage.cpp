
#include "ProtoBufRemote/RpcMessage.h"

#include <string.h>


namespace ProtoBufRemote
{
    
void CopyParameterMessage(const RpcMessageParameter& source, RpcMessageParameter* dest)
{
    *dest = source;

    if (source.string_param)
    {
        size_t len = strlen(source.string_param);
        dest->string_param = new char[len+1];
        strcpy(dest->string_param, source.string_param);
    }

    if (source.proto_param.data)
    {
        dest->proto_param.data = new uint8_t[source.proto_param.len];
        memcpy(dest->proto_param.data, source.proto_param.data, source.proto_param.len);
    }
}

void FreeParameterMessageFields(RpcMessageParameter* paramMessage)
{
    //free fields that may have been allocated. NOT unpacked fields, fields must have been allocated by us

    delete[] paramMessage->string_param;
    paramMessage->string_param = NULL;

    delete[] paramMessage->proto_param.data;
    paramMessage->proto_param.data = NULL;
    paramMessage->proto_param.len = 0;
}

}

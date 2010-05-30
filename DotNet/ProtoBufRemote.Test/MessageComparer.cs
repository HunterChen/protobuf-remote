using System.IO;
using System.Linq;
using ProtoBuf;

namespace ProtoBufRemote.Test
{
    public static class MessageComparer
    {
        public static bool IsEquivalentTo(this RpcMessage message, RpcMessage otherMessage)
        {
            if (message.Id != otherMessage.Id)
                return false;
            if ((message.CallMessage == null && otherMessage.CallMessage != null)
                || (message.CallMessage != null && otherMessage.CallMessage == null))
                return false;
            if ((message.ResultMessage == null && otherMessage.ResultMessage != null)
                || (message.ResultMessage != null && otherMessage.ResultMessage == null))
                return false;
            if (message.CallMessage != null)
            {
                if (!message.CallMessage.IsEquivalentTo(otherMessage.CallMessage))
                    return false;
            }
            if (message.ResultMessage != null)
            {
                if (!message.ResultMessage.IsEquivalentTo(otherMessage.ResultMessage))
                    return false;
            }
            return true;
        }

        public static bool IsEquivalentTo(this RpcMessage.Call message, RpcMessage.Call otherMessage)
        {
            if (message.Service != otherMessage.Service|| message.Method != otherMessage.Method
                || message.ExpectsResult != otherMessage.ExpectsResult)
                return false;
            if (message.Parameters.Count != otherMessage.Parameters.Count)
                return false;
            for (int i = 0; i < message.Parameters.Count; ++i )
            {
                if (!message.Parameters[i].IsEquivalentTo(otherMessage.Parameters[i]))
                    return false;
            }
            return true;
        }

        public static bool IsEquivalentTo(this RpcMessage.Result message, RpcMessage.Result otherMessage)
        {
            if (!message.CallResult.IsEquivalentTo(otherMessage.CallResult))
                return false;
            return true;
        }

        public static bool IsEquivalentTo(this RpcMessage.Parameter message, RpcMessage.Parameter otherMessage)
        {
            if (!message.ProtoParam.SequenceEqual(otherMessage.ProtoParam))
                return false;
            return true;
        }

        public static bool IsEquivalentTo(this MultiplyInput message, MultiplyInput otherMessage)
        {
            if (message.FirstNumber != otherMessage.FirstNumber || message.SecondNumber != otherMessage.SecondNumber
                || message.Prefix != otherMessage.Prefix)
                return false;
            return true;
        }

        public static bool IsEquivalentTo(this MultiplyOutput message, MultiplyOutput otherMessage)
        {
            if (message.ResultNumber != otherMessage.ResultNumber || message.ResultString != otherMessage.ResultString)
                return false;
            return true;
        }

        public static bool IsEquivalentTo(this MultiplyInput message, byte[] otherMessageBytes)
        {
            var memStream = new MemoryStream(otherMessageBytes);
            MultiplyInput otherMessage = Serializer.Deserialize<MultiplyInput>(memStream);
            return message.IsEquivalentTo(otherMessage);
        }
    }
}

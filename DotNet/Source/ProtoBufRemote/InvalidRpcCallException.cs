using System;

namespace ProtoBufRemote
{
    public class InvalidRpcCallException : Exception
    {
        public InvalidRpcCallException(string service, string method, string message)
            : base(String.Format("Service: {0}, Method: {1} - {2}", service, method, message))
        {
        }
    }
}

using System.Net.Sockets;

namespace ProtoBufRemote
{
    /// <summary>
    /// Trivial subclass of StreamRpcChannel, uses the NetworkStream as both the read stream and the write stream.
    /// </summary>
    public class NetworkStreamRpcChannel : StreamRpcChannel
    {
        public NetworkStreamRpcChannel(RpcController controller, NetworkStream stream)
            : base(controller, stream, stream)
        {
        }
    }
}

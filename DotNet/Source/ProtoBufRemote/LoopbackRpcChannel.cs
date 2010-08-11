
namespace ProtoBufRemote
{
    /// <summary>
    /// A dummy RPC channel which just returns all sent messages to itself, useful for testing.
    /// </summary>
    public class LoopbackRpcChannel : RpcChannel
    {
        public LoopbackRpcChannel(RpcController controller) : base(controller)
        {
        }

        internal override void Send(RpcMessage message)
        {
            Receive(message);
        }
    }
}

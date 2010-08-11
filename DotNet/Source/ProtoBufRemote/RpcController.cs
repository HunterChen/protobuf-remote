
namespace ProtoBufRemote
{
    public class RpcController
    {
        public virtual RpcClient Client { get; internal set; }

        public virtual RpcServer Server { get; internal set; }

        public virtual RpcChannel Channel { get; internal set; }

        internal virtual void Send(RpcMessage message)
        {
            Channel.Send(message);
        }

        internal virtual void Receive(RpcMessage message)
        {
            if (message.CallMessage != null)
                Server.ReceiveCall(message);
            else
                Client.ReceiveResult(message);
        }

    }
}


namespace ProtoBufRemote
{
    public abstract class RpcChannel
    {
        private readonly RpcController controller;

        protected RpcChannel(RpcController controller)
        {
            this.controller = controller;
            controller.Channel = this;
        }

        public virtual void Start()
        {
        }

        internal abstract void Send(RpcMessage message);

        /// <summary>
        /// Handles a message which has been received. Can be called by multiple threads concurrently, it is up to the
        /// RpcClient and RpcServer to ensure they are thread-safe.
        /// </summary>
        /// <returns></returns>
        protected void Receive(RpcMessage message)
        {
            controller.Receive(message);
        }
    }
}

using Moq;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    class LoopbackRpcChannelTest
    {
        private Mock<RpcController> controller;
        private LoopbackRpcChannel channel;

        [SetUp]
        public void Init()
        {
            controller = new Mock<RpcController>();
            channel = new LoopbackRpcChannel(controller.Object);
        }

        [Test]
        public void ControllerChannelSet()
        {
            controller.VerifySet(c => c.Channel = channel);
        }

        [Test]
        public void SendAndReceive()
        {
            var message = new RpcMessage();

            channel.Send(message);

            controller.Verify(c => c.Receive(message));
        }
    }
}

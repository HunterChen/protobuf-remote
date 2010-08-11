using Moq;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class RpcControllerTest
    {
        private Mock<RpcChannel> channel;
        private Mock<RpcClient> client;
        private Mock<RpcServer> server;
        private RpcController controller;

        [SetUp]
        public void Init()
        {

            controller = new RpcController();
            client = new Mock<RpcClient>(new object[] { controller });
            server = new Mock<RpcServer>(new object[] { controller });
            channel = new Mock<RpcChannel>(new object[] { controller });
            controller.Client = client.Object;
            controller.Server = server.Object;
            controller.Channel = channel.Object;
        }

        [Test]
        public void Send()
        {
            var message = new RpcMessage();
            controller.Send(message);

            channel.Verify(c => c.Send(message));
        }

        [Test]
        public void ReceiveCall()
        {
            var message = new RpcMessage();
            message.CallMessage = new RpcMessage.Call();
            controller.Receive(message);

            server.Verify(s => s.ReceiveCall(message));
        }

        [Test]
        public void ReceiveResult()
        {
            var message = new RpcMessage();
            message.ResultMessage = new RpcMessage.Result();
            controller.Receive(message);

            client.Verify(c => c.ReceiveResult(message));
        }
    }
}

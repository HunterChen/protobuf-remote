using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class ClientServerLoopbackTest
    {
        private RpcClient client;
        private RpcServer server;
        private dynamic dynProxy;

        [SetUp]
        public void Init()
        {
            var controller = new RpcController();
            client = new RpcClient(controller);
            server = new RpcServer(controller);
            server.RegisterService(new SampleService());
            var channel = new LoopbackRpcChannel(controller);
            channel.Start();

            dynProxy = client.GetProxy("ISampleService");
        }

        [Test]
        public void CallTest()
        {
            int result = dynProxy.GetSquare(5);
            Assert.That(result, Is.EqualTo(25));
        }
    }
}

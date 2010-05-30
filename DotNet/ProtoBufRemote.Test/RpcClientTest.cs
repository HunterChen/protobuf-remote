using Moq;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class RpcClientTest
    {
        private Mock<RpcController> controller;
        private RpcClient client;
        private RpcMessage defaultResult;

        [SetUp]
        public void Init()
        {
            controller = new Mock<RpcController>();
            client = new RpcClient(controller.Object);

            defaultResult = new RpcMessage();
            defaultResult.ResultMessage = new RpcMessage.Result();
            defaultResult.ResultMessage.CallResult = new RpcMessage.Parameter();
            defaultResult.ResultMessage.CallResult.IntParam = 100;

            controller.Setup(c => c.Send(It.IsAny<RpcMessage>()))
                .Callback((RpcMessage m) =>
                {
                    defaultResult.Id = m.Id;
                    if (m.CallMessage.ExpectsResult)
                        client.ReceiveResult(defaultResult);
                });
        }

        [Test]
        public void ControllerSetTest()
        {
            controller.VerifySet(c => c.Client = client);
        }

        [Test]
        public void GetDuplicateProxyTest()
        {
            ISampleService service1 = client.GetProxy<ISampleService>();
            ISampleService service2 = client.GetProxy<ISampleService>();
            Assert.That(service1.GetType(), Is.EqualTo(service2.GetType()));
        }

        [Test]
        public void CallThroughProxyTest()
        {
            ISampleService service = client.GetProxy<ISampleService>();
            int x = service.GetSquare(10);

            Assert.That(x, Is.EqualTo(100));
        }

        [Test]
        public void CallThroughDynamicProxyTest()
        {
            dynamic service = client.GetProxy("ISampleService");
            int x = service.GetSquare(10);

            Assert.That(x, Is.EqualTo(100));
        }

        [Test]
        public void CallTest()
        {
            PendingCall pendingCall = client.Call("ServiceName", "MethodName", null, null, null);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => m.Id == pendingCall.Id
                && m.CallMessage.Service == "ServiceName" && m.CallMessage.Method == "MethodName"
                && m.CallMessage.ExpectsResult)));
        }

        [Test]
        public void CallWithoutResultTest()
        {
            client.CallWithoutResult("ServiceName", "MethodName", null);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => m.CallMessage.Service == "ServiceName"
                && m.CallMessage.Method == "MethodName" && !m.CallMessage.ExpectsResult)));
        }
    }
}

using System;
using Moq;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class DynamicProxyTest
    {
        private Mock<RpcClient> client;

        [SetUp]
        public void Init()
        {
            client = new Mock<RpcClient>(new Mock<RpcController>().Object);

            var pendingCall = new PendingCall(0, "", "", null, null);
            pendingCall.ReceiveResult(
                new RpcMessage.Result { CallResult = new RpcMessage.Parameter { IntParam = 42 } });

            client.Setup(c =>
                c.Call(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<AsyncCallback>(),
                    It.IsAny<object>())).Returns(pendingCall);
        }

        [Test]
        public void CallTest()
        {
            dynamic serviceProxy = new DynamicProxy(client.Object, "TestServiceName");
            serviceProxy.TestMethod(10, "Hello", 1.0f);

            client.Verify(c => c.Call("TestServiceName", "TestMethod", new object[] { 10, "Hello", 1.0f }, null, null));
        }

        [Test]
        public void CallWithResultTest()
        {
            dynamic serviceProxy = new DynamicProxy(client.Object, "TestServiceName");
            int result = serviceProxy.TestMethod(10, "Hello", 1.0f);

            Assert.That(result, Is.EqualTo(42));
            client.Verify(c => c.Call("TestServiceName", "TestMethod", new object[] { 10, "Hello", 1.0f }, null, null));
        }

        [Test, ExpectedException(typeof(InvalidRpcCallException))]
        public void CallResultTypeFailedTest()
        {
            dynamic serviceProxy = new DynamicProxy(client.Object, "TestServiceName");
            MultiplyOutput result = serviceProxy.TestMethod(10, "Hello", 1.0f);
        }

        [Test, ExpectedException(typeof(InvalidRpcCallException))]
        public void CallResultMissingFailedTest()
        {
            var pendingCall = new PendingCall(0, "", "", null, null);
            pendingCall.ReceiveResult(new RpcMessage.Result());

            client.Setup(c =>
                c.Call(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<AsyncCallback>(),
                    It.IsAny<object>())).Returns(pendingCall);

            dynamic serviceProxy = new DynamicProxy(client.Object, "TestServiceName");
            int x = serviceProxy.TestMethod(10, "Hello", 1.0f);
        }

        [Test, ExpectedException(typeof(InvalidRpcCallException))]
        public void CallFailedTest()
        {
            var pendingCall = new PendingCall(0, "", "", null, null);
            pendingCall.ReceiveResult(new RpcMessage.Result { IsFailed = true });

            client.Setup(c =>
                c.Call(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<AsyncCallback>(),
                    It.IsAny<object>())).Returns(pendingCall);

            dynamic serviceProxy = new DynamicProxy(client.Object, "TestServiceName");
            serviceProxy.TestMethod(10, "Hello", 1.0f);
        }

        [Test]
        public void AsyncCallbackTest()
        {
            bool isCallbackDone = false;
            AsyncCallback callback = ar => isCallbackDone = true;

            var pendingCall = new PendingCall(0, "", "", callback, this);
            pendingCall.ReceiveResult(
                new RpcMessage.Result { CallResult = new RpcMessage.Parameter { IntParam = 42 } });

            client.Setup(c =>
                c.Call(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<AsyncCallback>(),
                    It.IsAny<object>())).Returns(pendingCall);

            dynamic serviceProxy = new DynamicProxy(client.Object, "TestServiceName");
            IAsyncResult asyncResult = serviceProxy.BeginTestMethod("param", 42.0f, callback, this);
            Assert.That(asyncResult.AsyncState, Is.EqualTo(this));
            asyncResult.AsyncWaitHandle.WaitOne(); //not necessary, just because i can
            int result = serviceProxy.EndTestMethod(asyncResult);

            Assert.That(isCallbackDone);
            Assert.That(asyncResult.IsCompleted);
            Assert.That(result, Is.EqualTo(42));
            client.Verify(c => c.Call("TestServiceName", "TestMethod", new object[] { "param", 42.0f }, callback, this));
        }
    }
}

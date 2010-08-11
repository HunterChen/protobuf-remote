using System;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class ProxyTest
    {
        private ProxyBuilder proxyBuilder;
        private Mock<RpcClient> client;
        private PendingCall pendingSquareCall;

        [SetUp]
        public void Init()
        {
            proxyBuilder = new ProxyBuilder();
            client = new Mock<RpcClient>(new Mock<RpcController>().Object);

            pendingSquareCall = new PendingCall(0, null, null, null, null);
            var squareResultMessage = new RpcMessage.Result();
            squareResultMessage.CallResult = new RpcMessage.Parameter();
            squareResultMessage.CallResult.IntParam = 42;
            pendingSquareCall.ReceiveResult(squareResultMessage);

            client.Setup(c => c.Call("ISampleService", "GetSquare", It.IsAny<RpcMessage.Parameter[]>(),
                It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Returns(pendingSquareCall);
        }

        class NotAnInterfaceService
        {
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void NotAnInterfaceFailureTest()
        {
            proxyBuilder.Build(typeof(NotAnInterfaceService));
        }

        [Test]
        public void CallTest()
        {
            Type proxyType = proxyBuilder.Build(typeof(ISampleService));
            ISampleService service = (ISampleService)Activator.CreateInstance(proxyType, client.Object);
            int x = service.GetSquare(10);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "GetSquare",
                It.Is<RpcMessage.Parameter[]>(p => IsSquareParamArrayCorrect(p)), null, null));
        }

        [Test]
        public void CallWithoutResultTest()
        {
            Type proxyType = proxyBuilder.Build(typeof(ISampleService));
            ISampleService service = (ISampleService)Activator.CreateInstance(proxyType, client.Object);
            service.DoStuff("test");

            client.Verify(c => c.CallWithoutResult("ISampleService", "DoStuff", It.IsAny<RpcMessage.Parameter[]>()));
        }

        [Test]
        public void AsyncTest()
        {
            Type proxyType = proxyBuilder.Build(typeof(ISampleServiceAsync));
            ISampleServiceAsync service = (ISampleServiceAsync)Activator.CreateInstance(proxyType, client.Object);

            IAsyncResult asyncResult = service.BeginGetSquare(10, null, this);
            asyncResult.AsyncWaitHandle.WaitOne(); //not necessary, just because i can
            int result = service.EndGetSquare(asyncResult);

            Assert.That(asyncResult.IsCompleted);
            Assert.That(result, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "GetSquare",
                It.Is<RpcMessage.Parameter[]>(p => IsSquareParamArrayCorrect(p)), null, this));
        }

        private static bool IsSquareParamArrayCorrect(RpcMessage.Parameter[] parameters)
        {
            if (parameters.Length != 1)
                return false;
            if (parameters[0].IntParam != 10)
                return false;
            return true;
        }
    }

    [TestFixture]
    public class ProxyParameterTest
    {
        private ProxyBuilder proxyBuilder;
        private Mock<RpcClient> client;
        private ISampleService service;
        private RpcMessage.Result resultMessage;
        private PendingCall pendingCall;

        [SetUp]
        public void Init()
        {
            proxyBuilder = new ProxyBuilder();
            client = new Mock<RpcClient>(new Mock<RpcController>().Object);

            Type proxyType = proxyBuilder.Build(typeof(ISampleService));
            service = (ISampleService)Activator.CreateInstance(proxyType, client.Object);

            pendingCall = new PendingCall(0, null, null, null, null);
            resultMessage = new RpcMessage.Result();
            resultMessage.CallResult = new RpcMessage.Parameter();
            pendingCall.ReceiveResult(resultMessage);

            client.Setup(c => c.Call(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RpcMessage.Parameter[]>(),
                It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Returns(pendingCall);
        }

        [Test]
        public void ParameterByteTest()
        {
            resultMessage.CallResult.UintParam = 42;
            byte x = service.TestByteParam(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestByteParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].UintParam == 5), null, null));
        }

        [Test]
        public void ParameterSbyteTest()
        {
            resultMessage.CallResult.IntParam = 42;
            sbyte x = service.TestSbyteParam(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestSbyteParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].IntParam == 5), null, null));
        }

        [Test]
        public void ParameterCharTest()
        {
            resultMessage.CallResult.UintParam = 42;
            char x = service.TestCharParam((char)5);

            Assert.That(x, Is.EqualTo((char)42));
            client.Verify(c => c.Call("ISampleService", "TestCharParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].UintParam == 5), null, null));
        }

        [Test]
        public void ParameterShortTest()
        {
            resultMessage.CallResult.IntParam = 42;
            short x = service.TestShortParam(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestShortParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].IntParam == 5), null, null));
        }

        [Test]
        public void ParameterUshortTest()
        {
            resultMessage.CallResult.UintParam = 42;
            ushort x = service.TestUshortParam(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestUshortParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].UintParam == 5), null, null));
        }

        [Test]
        public void ParameterIntTest()
        {
            resultMessage.CallResult.IntParam = 42;
            int x = service.TestIntParam(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestIntParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].IntParam == 5), null, null));
        }

        [Test]
        public void ParameterUintTest()
        {
            resultMessage.CallResult.UintParam = 42;
            uint x = service.TestUintParam(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestUintParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].UintParam == 5), null, null));
        }

        [Test]
        public void ParameterInt64Test()
        {
            resultMessage.CallResult.Int64Param = 42;
            long x = service.TestInt64Param(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestInt64Param",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].Int64Param == 5), null, null));
        }

        [Test]
        public void ParameterUint64Test()
        {
            resultMessage.CallResult.Uint64Param = 42;
            ulong x = service.TestUint64Param(5);

            Assert.That(x, Is.EqualTo(42));
            client.Verify(c => c.Call("ISampleService", "TestUint64Param",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].Uint64Param == 5), null, null));
        }

        [Test]
        public void ParameterBoolTest()
        {
            resultMessage.CallResult.BoolParam = true;
            bool x = service.TestBoolParam(true);

            Assert.That(x, Is.True);
            client.Verify(c => c.Call("ISampleService", "TestBoolParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].BoolParam), null, null));
        }

        [Test]
        public void ParameterFloatTest()
        {
            resultMessage.CallResult.FloatParam = 42.0f;
            float x = service.TestFloatParam(5.0f);

            Assert.That(x, Is.EqualTo(42.0f));
            client.Verify(c => c.Call("ISampleService", "TestFloatParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].FloatParam == 5.0f), null, null));
        }

        [Test]
        public void ParameterDoubleTest()
        {
            resultMessage.CallResult.DoubleParam = 42.0;
            double x = service.TestDoubleParam(5.0);

            Assert.That(x, Is.EqualTo(42.0));
            client.Verify(c => c.Call("ISampleService", "TestDoubleParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].DoubleParam == 5.0), null, null));
        }

        [Test]
        public void ParameterStringTest()
        {
            resultMessage.CallResult.StringParam = "result";
            string x = service.TestStringParam("input");

            Assert.That(x, Is.EqualTo("result"));
            client.Verify(c => c.Call("ISampleService", "TestStringParam",
                It.Is<RpcMessage.Parameter[]>(p => p.Length == 1 && p[0].StringParam == "input"), null, null));
        }

        [Test]
        public void ParameterProtoTest()
        {
            MultiplyInput input = new MultiplyInput();
            input.FirstNumber = 6;
            input.SecondNumber = 7;
            input.Prefix = "the prefix";
            var memInputStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memInputStream, input);

            MultiplyOutput output = new MultiplyOutput();
            output.ResultNumber = 42;
            output.ResultString = "the result";
            var memOutputStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memOutputStream, output);

            resultMessage.CallResult.ProtoParam = memOutputStream.ToArray();
            MultiplyOutput x = service.Multiply(input);

            Assert.That(x.ResultNumber, Is.EqualTo(42));
            Assert.That(x.ResultString, Is.EqualTo("the result"));
            client.Verify(c => c.Call("ISampleService", "Multiply", It.Is<RpcMessage.Parameter[]>(
                p => p.Length == 1 && p[0].ProtoParam.SequenceEqual(memInputStream.ToArray())), null, null));
        }
    }
}

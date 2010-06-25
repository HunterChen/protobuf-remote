using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class RpcServerTest
    {
        private RpcServer server;
        private Mock<RpcController> controller;
        private RpcMessage squareCallMessage;
        private RpcMessage doStuffCallMessage;
        private RpcMessage testParamCallMessage;
        private SampleService sampleService;

        [SetUp]
        public void Init()
        {
            controller = new Mock<RpcController>();

            squareCallMessage = new RpcMessage();
            squareCallMessage.Id = 42;
            squareCallMessage.CallMessage = new RpcMessage.Call();
            squareCallMessage.CallMessage.Service = "ISampleService";
            squareCallMessage.CallMessage.Method = "GetSquare";
            squareCallMessage.CallMessage.ExpectsResult = true;
            squareCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { IntParam = 5 });

            doStuffCallMessage = new RpcMessage();
            doStuffCallMessage.Id = 43;
            doStuffCallMessage.CallMessage = new RpcMessage.Call();
            doStuffCallMessage.CallMessage.Service = "ISampleService";
            doStuffCallMessage.CallMessage.Method = "DoStuff";
            doStuffCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { StringParam = "Hello" });

            testParamCallMessage = new RpcMessage();
            testParamCallMessage.Id = 44;
            testParamCallMessage.CallMessage = new RpcMessage.Call();
            testParamCallMessage.CallMessage.Service = "ISampleService";
            testParamCallMessage.CallMessage.ExpectsResult = true;
            
            server = new RpcServer(controller.Object);

            sampleService = new SampleService();
            server.RegisterService(sampleService);
        }

        [Test]
        public void ControllerSetTest()
        {
            controller.VerifySet(c => c.Server = server);
        }

        interface IServiceTestInterface
        {
            int DoStuff();
        }
        class ServiceTestImpl : IServiceTestInterface
        {
            public int DoStuff() { return 42; }
        }

        [Test]
        public void RegisterServiceTest()
        {
            //this should pass, we explicitly specify the interface as the service type
            server.RegisterService<IServiceTestInterface>(new ServiceTestImpl());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void RegisterServiceFailureTest()
        {
            //this should fail, a class type cannot be used as a service unless it is attributed
            server.RegisterService(new ServiceTestImpl());
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void RegisterDuplicatedServiceFailureTest()
        {
            server.RegisterService<IServiceTestInterface>(new ServiceTestImpl());
            server.RegisterService<IServiceTestInterface>(new ServiceTestImpl());
        }

        [Test]
        public void CallTest()
        {
            server.ReceiveCall(squareCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsSquareResultMessageCorrect(m))));
        }

        [Test]
        public void CallWithoutResultTest()
        {
            squareCallMessage.CallMessage.ExpectsResult = false;
            server.ReceiveCall(squareCallMessage);

            controller.Verify(c => c.Send(It.IsAny<RpcMessage>()), Times.Never());
        }

        [Test]
        public void CallUnknownServiceTest()
        {
            squareCallMessage.CallMessage.Service = "NotAnActualService";
            server.ReceiveCall(squareCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsFailureResultMessage(m))));
        }

        [Test]
        public void CallUnknownMethodTest()
        {
            squareCallMessage.CallMessage.Method = "NotAnActualMethod";
            server.ReceiveCall(squareCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsFailureResultMessage(m))));
        }

        [Test]
        public void CallVoidReturnMethodTest()
        {
            doStuffCallMessage.CallMessage.ExpectsResult = true;
            server.ReceiveCall(doStuffCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsDoStuffResultMessageCorrect(m))));
            Assert.That(sampleService.NumTimesStuffDone, Is.EqualTo(1));
        }

        [Test]
        public void CallVoidReturnMethodWithoutResultTest()
        {
            doStuffCallMessage.CallMessage.ExpectsResult = false;
            server.ReceiveCall(doStuffCallMessage);

            controller.Verify(c => c.Send(It.IsAny<RpcMessage>()), Times.Never());
            Assert.That(sampleService.NumTimesStuffDone, Is.EqualTo(1));
        }

        [Test]
        public void WrongParameterCountTest()
        {
            var extraParam = new RpcMessage.Parameter();
            extraParam.IntParam = 10;
            squareCallMessage.CallMessage.Parameters.Add(extraParam);
            server.ReceiveCall(squareCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsFailureResultMessage(m))));
        }

        [Test]
        public void WrongParameterTypeTest()
        {
            squareCallMessage.CallMessage.Parameters[0].IntParamSpecified = false;
            squareCallMessage.CallMessage.Parameters[0].UintParam = 20;
            server.ReceiveCall(squareCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsFailureResultMessage(m))));
        }

        [Test]
        public void ParameterByteTest()
        {
            testParamCallMessage.CallMessage.Method = "TestByteParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { UintParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c =>c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.UintParam == 43)));
        }

        [Test]
        public void ParameterSbyteTest()
        {
            testParamCallMessage.CallMessage.Method = "TestSbyteParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { IntParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.IntParam == 43)));
        }

        [Test]
        public void ParameterCharTest()
        {
            testParamCallMessage.CallMessage.Method = "TestCharParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { UintParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.UintParam == 43)));
        }

        [Test]
        public void ParameterShortTest()
        {
            testParamCallMessage.CallMessage.Method = "TestShortParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { IntParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.IntParam == 43)));
        }

        [Test]
        public void ParameterUshortTest()
        {
            testParamCallMessage.CallMessage.Method = "TestUshortParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { UintParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.UintParam == 43)));
        }

        [Test]
        public void ParameterIntTest()
        {
            testParamCallMessage.CallMessage.Method = "TestIntParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { IntParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.IntParam == 43)));
        }

        [Test]
        public void ParameterUintTest()
        {
            testParamCallMessage.CallMessage.Method = "TestUintParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { UintParam = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.UintParam == 43)));
        }

        [Test]
        public void ParameterInt64Test()
        {
            testParamCallMessage.CallMessage.Method = "TestInt64Param";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { Int64Param = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.Int64Param == 43)));
        }

        [Test]
        public void ParameterUint64Test()
        {
            testParamCallMessage.CallMessage.Method = "TestUint64Param";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { Uint64Param = 42 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.Uint64Param == 43)));
        }

        [Test]
        public void ParameterBoolTest()
        {
            testParamCallMessage.CallMessage.Method = "TestBoolParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { BoolParam = true });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.BoolParam)));
        }

        [Test]
        public void ParameterFloatTest()
        {
            testParamCallMessage.CallMessage.Method = "TestFloatParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { FloatParam = 42.0f });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.FloatParam == 43.0f)));
        }

        [Test]
        public void ParameterDoubleTest()
        {
            testParamCallMessage.CallMessage.Method = "TestDoubleParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { DoubleParam = 42.0 });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.DoubleParam == 43.0)));
        }

        [Test]
        public void ParameterStringTest()
        {
            testParamCallMessage.CallMessage.Method = "TestStringParam";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter { StringParam = "Hello" });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(
                m => IsTestParamResultMessageCorrect(m) && m.ResultMessage.CallResult.StringParam == "Hello world")));
        }

        [Test]
        public void ParameterProtoTest()
        {
            var input = new MultiplyInput();
            input.FirstNumber = 6;
            input.SecondNumber = 7;
            input.Prefix = "Answer is ";
            
            var memStream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(memStream, input);

            testParamCallMessage.CallMessage.Method = "Multiply";
            testParamCallMessage.CallMessage.Parameters.Add(new RpcMessage.Parameter
                                                            { ProtoParam = memStream.ToArray() });
            server.ReceiveCall(testParamCallMessage);

            controller.Verify(c => c.Send(It.Is<RpcMessage>(m => IsTestProtoParamResultMessageCorrect(m))));
        }

        private bool IsSquareResultMessageCorrect(RpcMessage message)
        {
            if (message == null || message.Id != squareCallMessage.Id || message.ResultMessage == null)
                return false;
            if (message.ResultMessage.IsFailed || message.ResultMessage.CallResult == null)
                return false;
            if (message.ResultMessage.CallResult.IntParam != 25)
                return false;
            return true;
        }

        private bool IsFailureResultMessage(RpcMessage message)
        {
            if (message == null || message.Id != squareCallMessage.Id || message.ResultMessage == null)
                return false;
            if (!message.ResultMessage.IsFailed)
                return false;
            return true;
        }

        private bool IsDoStuffResultMessageCorrect(RpcMessage message)
        {
            if (message == null || message.Id != doStuffCallMessage.Id || message.ResultMessage == null)
                return false;
            if (message.ResultMessage.IsFailed)
                return false;
            if (message.ResultMessage.CallResult != null)
                return false;
            return true;
        }

        private bool IsTestParamResultMessageCorrect(RpcMessage message)
        {
            if (message == null || message.Id != testParamCallMessage.Id || message.ResultMessage == null)
                return false;
            if (message.ResultMessage.IsFailed || message.ResultMessage.CallResult == null)
                return false;
            return true;
        }

        private bool IsTestProtoParamResultMessageCorrect(RpcMessage message)
        {
            if (message == null || message.Id != testParamCallMessage.Id || message.ResultMessage == null)
                return false;
            if (message.ResultMessage.IsFailed || message.ResultMessage.CallResult == null)
                return false;

            var memStream = new MemoryStream(message.ResultMessage.CallResult.ProtoParam);
            MultiplyOutput output = ProtoBuf.Serializer.Deserialize<MultiplyOutput>(memStream);
            if (output.ResultNumber != 42 || output.ResultString != "Answer is 42")
                return false;

            return true;
        }
    }
}

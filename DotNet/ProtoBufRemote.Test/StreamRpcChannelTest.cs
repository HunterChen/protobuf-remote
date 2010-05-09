using System.IO;
using System.Threading;
using Moq;
using NUnit.Framework;
using ProtoBuf;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class StreamRpcChannelTest
    {
        private Mock<RpcController> controller;
        private MemoryStream readStream;
        private MemoryStream writeStream;
        private RpcMessage testMessage;

        [SetUp]
        public void Init()
        {
            controller = new Mock<RpcController>();
            readStream = new MemoryStream();
            writeStream = new MemoryStream();

            testMessage = new RpcMessage();
            testMessage.Id = 42;
            testMessage.CallMessage = new RpcMessage.Call();
            testMessage.CallMessage.Service = "testService";
            testMessage.CallMessage.Method = "testMethod";
            var param = new RpcMessage.Parameter();
            param.ProtoParam = new byte[] { 1, 2, 3 };
            testMessage.CallMessage.Parameters.Add(param);
        }

        [Test]
        public void ControllerChannelSet()
        {
            var channel = new StreamRpcChannel(controller.Object, readStream, writeStream);
            controller.VerifySet(c => c.Channel = channel);
        }

        [Test]
        public void Send()
        {
            var channel = new StreamRpcChannel(controller.Object, readStream, writeStream);
            channel.Start();
            channel.Send(testMessage);
            channel.CloseAndJoin(false);

            writeStream.Seek(0, SeekOrigin.Begin);
            var receivedMessage = Serializer.DeserializeWithLengthPrefix<RpcMessage>(writeStream, PrefixStyle.Fixed32);

            Assert.That(receivedMessage.IsEquivalentTo(testMessage), Is.True);
        }

        [Test]
        public void Receive()
        {
            Serializer.SerializeWithLengthPrefix(readStream, testMessage, PrefixStyle.Fixed32);
            readStream.Seek(0, SeekOrigin.Begin);

            ManualResetEvent doneEvent = new ManualResetEvent(false);
            controller.Setup(c => c.Receive(It.Is<RpcMessage>(m => m.IsEquivalentTo(testMessage))))
                .Callback(() => doneEvent.Set());

            var channel = new StreamRpcChannel(controller.Object, readStream, writeStream);
            channel.Start();
            doneEvent.WaitOne(2000);
            channel.CloseAndJoin();

            controller.VerifyAll();
        }

    }
}

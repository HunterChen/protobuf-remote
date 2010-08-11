using System.IO;
using System.Linq;
using NUnit.Framework;
using ProtoBuf;

namespace ProtoBufRemote.Test
{
    [TestFixture]
    public class ParameterConverterTest
    {
        private RpcMessage.Parameter message;
        string errorMsg;
        object param;

        [SetUp]
        public void Init()
        {
            message = new RpcMessage.Parameter();
        }

        //======================================================================================================
        //======================================================================================================

        [Test]
        public void FromMessageBoolTest()
        {
            bool boolParam = false;
            Assert.That(ParameterConverter.FromMessage(message, typeof(bool), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.BoolFromMessage(message, ref boolParam, ref errorMsg), Is.False);
            message.BoolParam = true;
            Assert.That(ParameterConverter.FromMessage(message, typeof(bool), out param, out errorMsg), Is.True);
            Assert.That(param, Is.True);
            Assert.That(ParameterConverter.BoolFromMessage(message, ref boolParam, ref errorMsg), Is.True);
            Assert.That(boolParam, Is.True);
        }

        [Test]
        public void FromMessageByteTest()
        {
            Assert.That(ParameterConverter.FromMessage(message, typeof(byte), out param, out errorMsg), Is.False);
            message.UintParam = 42;
            Assert.That(ParameterConverter.FromMessage(message, typeof(byte), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(42));
        }

        [Test]
        public void FromMessageSbyteTest()
        {
            Assert.That(ParameterConverter.FromMessage(message, typeof(sbyte), out param, out errorMsg), Is.False);
            message.IntParam = -42;
            Assert.That(ParameterConverter.FromMessage(message, typeof(sbyte), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(-42));
        }

        [Test]
        public void FromMessageCharTest()
        {
            Assert.That(ParameterConverter.FromMessage(message, typeof(char), out param, out errorMsg), Is.False);
            message.UintParam = 'a';
            Assert.That(ParameterConverter.FromMessage(message, typeof(char), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo('a'));
        }

        [Test]
        public void FromMessageShortTest()
        {
            Assert.That(ParameterConverter.FromMessage(message, typeof(short), out param, out errorMsg), Is.False);
            message.IntParam = -42;
            Assert.That(ParameterConverter.FromMessage(message, typeof(short), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(-42));
        }

        [Test]
        public void FromMessageUshortTest()
        {
            Assert.That(ParameterConverter.FromMessage(message, typeof(ushort), out param, out errorMsg), Is.False);
            message.UintParam = 42;
            Assert.That(ParameterConverter.FromMessage(message, typeof(ushort), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(42));
        }

        [Test]
        public void FromMessageIntTest()
        {
            int intParam = 0;
            Assert.That(ParameterConverter.FromMessage(message, typeof(int), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.IntFromMessage(message, ref intParam, ref errorMsg), Is.False);
            message.IntParam = -42;
            Assert.That(ParameterConverter.FromMessage(message, typeof(int), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(-42));
            Assert.That(ParameterConverter.IntFromMessage(message, ref intParam, ref errorMsg), Is.True);
            Assert.That(intParam, Is.EqualTo(-42));
        }

        [Test]
        public void FromMessageUintTest()
        {
            uint uintParam = 0;
            Assert.That(ParameterConverter.FromMessage(message, typeof(uint), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.UintFromMessage(message, ref uintParam, ref errorMsg), Is.False);
            message.UintParam = 42;
            Assert.That(ParameterConverter.FromMessage(message, typeof(uint), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(42));
            Assert.That(ParameterConverter.UintFromMessage(message, ref uintParam, ref errorMsg), Is.True);
            Assert.That(uintParam, Is.EqualTo(42));
        }

        [Test]
        public void FromMessageLongTest()
        {
            long longParam = 0;
            Assert.That(ParameterConverter.FromMessage(message, typeof(long), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.Int64FromMessage(message, ref longParam, ref errorMsg), Is.False);
            message.Int64Param = -42000000000;
            Assert.That(ParameterConverter.FromMessage(message, typeof(long), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(-42000000000));
            Assert.That(ParameterConverter.Int64FromMessage(message, ref longParam, ref errorMsg), Is.True);
            Assert.That(longParam, Is.EqualTo(-42000000000));
        }

        [Test]
        public void FromMessageUlongTest()
        {
            ulong ulongParam = 0;
            Assert.That(ParameterConverter.FromMessage(message, typeof(ulong), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.Uint64FromMessage(message, ref ulongParam, ref errorMsg), Is.False);
            message.Uint64Param = 42000000000;
            Assert.That(ParameterConverter.FromMessage(message, typeof(ulong), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(42000000000));
            Assert.That(ParameterConverter.Uint64FromMessage(message, ref ulongParam, ref errorMsg), Is.True);
            Assert.That(ulongParam, Is.EqualTo(42000000000));
        }

        [Test]
        public void FromMessageFloatTest()
        {
            float floatParam = 0.0f;
            Assert.That(ParameterConverter.FromMessage(message, typeof(float), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.FloatFromMessage(message, ref floatParam, ref errorMsg), Is.False);
            message.FloatParam = 42.0f;
            Assert.That(ParameterConverter.FromMessage(message, typeof(float), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(42.0f));
            Assert.That(ParameterConverter.FloatFromMessage(message, ref floatParam, ref errorMsg), Is.True);
            Assert.That(floatParam, Is.EqualTo(42.0f));
        }

        [Test]
        public void FromMessageDoubleTest()
        {
            double doubleParam = 0.0;
            Assert.That(ParameterConverter.FromMessage(message, typeof(double), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.DoubleFromMessage(message, ref doubleParam, ref errorMsg), Is.False);
            message.DoubleParam = 42.0;
            Assert.That(ParameterConverter.FromMessage(message, typeof(double), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(42.0));
            Assert.That(ParameterConverter.DoubleFromMessage(message, ref doubleParam, ref errorMsg), Is.True);
            Assert.That(doubleParam, Is.EqualTo(42.0));
        }

        [Test]
        public void FromMessageStringTest()
        {
            string stringParam = null;
            Assert.That(ParameterConverter.FromMessage(message, typeof(string), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.StringFromMessage(message, ref stringParam, ref errorMsg), Is.False);
            message.StringParam = "Hello";
            Assert.That(ParameterConverter.FromMessage(message, typeof(string), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo("Hello"));
            Assert.That(ParameterConverter.StringFromMessage(message, ref stringParam, ref errorMsg), Is.True);
            Assert.That(stringParam, Is.EqualTo("Hello"));
        }

        [Test]
        public void FromMessageNullStringTest()
        {
            message.IsNull = true;
            Assert.That(ParameterConverter.FromMessage(message, typeof(string), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(null));
        }

        [Test]
        public void FromMessageProtoTest()
        {
            Assert.That(ParameterConverter.FromMessage(message, typeof(MultiplyInput), out param, out errorMsg), Is.False);
            Assert.That(ParameterConverter.ProtoFromMessage(message, typeof(MultiplyInput), ref param, ref errorMsg), Is.False);

            var messageParam = new MultiplyInput();
            messageParam.FirstNumber = 10;
            messageParam.SecondNumber = 11;
            messageParam.Prefix = "Test";
            var memStream = new MemoryStream();
            Serializer.Serialize(memStream, messageParam);
            message.ProtoParam = memStream.ToArray();

            Assert.That(ParameterConverter.FromMessage(message, typeof(MultiplyInput), out param, out errorMsg), Is.True);
            MultiplyInput result = (MultiplyInput)param;
            Assert.That(result.FirstNumber, Is.EqualTo(10));
            Assert.That(result.SecondNumber, Is.EqualTo(11));
            Assert.That(result.Prefix, Is.EqualTo("Test"));

            Assert.That(ParameterConverter.ProtoFromMessage(message, typeof(MultiplyInput), ref param, ref errorMsg), Is.True);
            result = (MultiplyInput)param;
            Assert.That(result.FirstNumber, Is.EqualTo(10));
            Assert.That(result.SecondNumber, Is.EqualTo(11));
            Assert.That(result.Prefix, Is.EqualTo("Test"));
        }

        [Test]
        public void FromMessageNullProtoTest()
        {
            message.IsNull = true;
            Assert.That(ParameterConverter.FromMessage(message, typeof(MultiplyInput), out param, out errorMsg), Is.True);
            Assert.That(param, Is.EqualTo(null));
        }

        [Test]
        public void FromMessageNullValueTypeFailureTest()
        {
            message.IsNull = true;
            Assert.That(ParameterConverter.FromMessage(message, typeof(int), out param, out errorMsg), Is.False);
        }

        [Test]
        public void FromMessageUnsupportedValueTypeFailureTest()
        {
            message.IsNull = true;
            Assert.That(ParameterConverter.FromMessage(message, typeof(decimal), out param, out errorMsg), Is.False);
        }

        class NonProtoClass
        {
        }

        [Test]
        public void FromMessageNonProtoFailureTest()
        {
            message.ProtoParam = new byte[10];
            Assert.That(ParameterConverter.FromMessage(message, typeof(NonProtoClass), out param, out errorMsg), Is.False);
        }

        [Test]
        public void FromMessageProtoMissingFailureTest()
        {
            message.IntParam = 10;
            Assert.That(ParameterConverter.FromMessage(message, typeof(MultiplyInput), out param, out errorMsg), Is.False);
        }

        //======================================================================================================
        //======================================================================================================

        [Test]
        public void ToMessageNullTest()
        {
            Assert.That(ParameterConverter.ToMessage(null, out message, out errorMsg), Is.True);
            Assert.That(message.IsNull, Is.True);
        }

        [Test]
        public void ToMessageBoolTest()
        {
            Assert.That(ParameterConverter.ToMessage(true, out message, out errorMsg), Is.True);
            Assert.That(message.BoolParam, Is.True);
        }

        [Test]
        public void ToMessageSbyteTest()
        {
            Assert.That(ParameterConverter.ToMessage((sbyte)-42, out message, out errorMsg), Is.True);
            Assert.That(message.IntParam, Is.EqualTo(-42));
        }

        [Test]
        public void ToMessageByteTest()
        {
            Assert.That(ParameterConverter.ToMessage((byte)42, out message, out errorMsg), Is.True);
            Assert.That(message.UintParam, Is.EqualTo(42));
        }

        [Test]
        public void ToMessageCharTest()
        {
            Assert.That(ParameterConverter.ToMessage('a', out message, out errorMsg), Is.True);
            Assert.That((char)message.UintParam, Is.EqualTo('a'));
        }

        [Test]
        public void ToMessageShortTest()
        {
            Assert.That(ParameterConverter.ToMessage((short)-42, out message, out errorMsg), Is.True);
            Assert.That(message.IntParam, Is.EqualTo(-42));
        }

        [Test]
        public void ToMessageUshortTest()
        {
            Assert.That(ParameterConverter.ToMessage((ushort)42, out message, out errorMsg), Is.True);
            Assert.That(message.UintParam, Is.EqualTo(42));
        }

        [Test]
        public void ToMessageIntTest()
        {
            Assert.That(ParameterConverter.ToMessage((int)-42, out message, out errorMsg), Is.True);
            Assert.That(message.IntParam, Is.EqualTo(-42));
        }

        [Test]
        public void ToMessageUintTest()
        {
            Assert.That(ParameterConverter.ToMessage((uint)42, out message, out errorMsg), Is.True);
            Assert.That(message.UintParam, Is.EqualTo(42));
        }

        [Test]
        public void ToMessageInt64Test()
        {
            Assert.That(ParameterConverter.ToMessage((long)-42000000000, out message, out errorMsg), Is.True);
            Assert.That(message.Int64Param, Is.EqualTo(-42000000000));
        }

        [Test]
        public void ToMessageUint64Test()
        {
            Assert.That(ParameterConverter.ToMessage((ulong)42000000000, out message, out errorMsg), Is.True);
            Assert.That(message.Uint64Param, Is.EqualTo(42000000000));
        }

        [Test]
        public void ToMessageFloatTest()
        {
            Assert.That(ParameterConverter.ToMessage(42.0f, out message, out errorMsg), Is.True);
            Assert.That(message.FloatParam, Is.EqualTo(42.0f));
        }

        [Test]
        public void ToMessageDoubleTest()
        {
            Assert.That(ParameterConverter.ToMessage(42.0, out message, out errorMsg), Is.True);
            Assert.That(message.DoubleParam, Is.EqualTo(42.0));
        }

        [Test]
        public void ToMessageStringTest()
        {
            Assert.That(ParameterConverter.ToMessage("Hello", out message, out errorMsg), Is.True);
            Assert.That(message.StringParam, Is.EqualTo("Hello"));
        }

        [Test]
        public void ToMessageProtoTest()
        {
            var inputMessage = new MultiplyInput();
            inputMessage.FirstNumber = 20;
            inputMessage.SecondNumber = 21;
            inputMessage.Prefix = "Test";

            var memStream = new MemoryStream();
            Serializer.Serialize(memStream, inputMessage);
            
            Assert.That(ParameterConverter.ToMessage(inputMessage, out message, out errorMsg), Is.True);
            Assert.That(message.ProtoParam.SequenceEqual(memStream.ToArray()), Is.True);
        }

        [Test]
        public void ToMessageUnsupportedTypeTest()
        {
            Assert.That(!ParameterConverter.ToMessage((decimal)10.0, out message, out errorMsg), Is.True);
        }
    }
}

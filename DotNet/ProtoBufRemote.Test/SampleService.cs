using System;

namespace ProtoBufRemote.Test
{
    public class SampleService : ISampleService
    {
        public MultiplyOutput Multiply(MultiplyInput input)
        {
            var output = new MultiplyOutput();
            output.ResultNumber = input.FirstNumber*input.SecondNumber;
            output.ResultString = input.Prefix + output.ResultNumber;
            return output;
        }

        public int GetSquare(int x)
        {
            return x*x;
        }

        public string GetDate()
        {
            return DateTime.Now.ToLongDateString();
        }

        public void DoStuff(string data)
        {
            ++NumTimesStuffDone;
        }

        public int NumTimesStuffDone { get; set; }

        public byte TestByteParam(byte x) { return (byte)(x+1); }
        public sbyte TestSbyteParam(sbyte x) { return (sbyte)(x + 1); }
        public char TestCharParam(char x) { return (char)(x + 1); }
        public short TestShortParam(short x) { return (short)(x + 1); }
        public ushort TestUshortParam(ushort x) { return (ushort)(x + 1); }
        public int TestIntParam(int x) { return x + 1; }
        public uint TestUintParam(uint x) { return x + 1; }
        public long TestInt64Param(long x) { return x + 1; }
        public ulong TestUint64Param(ulong x) { return x + 1; }
        public bool TestBoolParam(bool x) { return x; }
        public float TestFloatParam(float x) { return x+1.0f; }
        public double TestDoubleParam(double x) { return x+1.0; }
        public string TestStringParam(string x) { return x+" world"; }
    }
}

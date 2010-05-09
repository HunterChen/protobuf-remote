using System;

namespace ProtoBufRemote.Test
{
    [RpcService]
    public interface ISampleService
    {
        MultiplyOutput Multiply(MultiplyInput input);

        int GetSquare(int x);

        string GetDate();

        void DoStuff(string data);

        byte TestByteParam(byte x);
        sbyte TestSbyteParam(sbyte x);
        char TestCharParam(char x);
        short TestShortParam(short x);
        ushort TestUshortParam(ushort x);
        int TestIntParam(int x);
        uint TestUintParam(uint x);
        long TestInt64Param(long x);
        ulong TestUint64Param(ulong x);
        bool TestBoolParam(bool x);
        float TestFloatParam(float x);
        double TestDoubleParam(double x);
        string TestStringParam(string x);
    }

    [RpcService(Name="ISampleService")]
    public interface ISampleServiceAsync : ISampleService
    {
        IAsyncResult BeginGetSquare(int x, AsyncCallback callback, object state);

        int EndGetSquare(IAsyncResult asyncResult);
    }
}

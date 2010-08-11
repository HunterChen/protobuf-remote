using System;
using System.Reflection.Emit;

namespace ProtoBufRemote
{
    internal static class ThunkHelpers
    {
        public static void EmitParameterToMessage(ILGenerator ilGen, Type paramType)
        {
            //stack already has a value of known type paramType
            if (paramType == typeof(int) || paramType == typeof(sbyte) || paramType == typeof(short))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("IntToMessage"));
            else if (paramType == typeof(uint) || paramType == typeof(byte) || paramType == typeof(char) || paramType == typeof(ushort))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("UintToMessage"));
            else if (paramType == typeof(long))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("Int64ToMessage"));
            else if (paramType == typeof(ulong))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("Uint64ToMessage"));
            else if (paramType == typeof(bool))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("BoolToMessage"));
            else if (paramType == typeof(float))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("FloatToMessage"));
            else if (paramType == typeof(double))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("DoubleToMessage"));
            else if (paramType == typeof(string))
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("StringToMessage"));
            else
                ilGen.Emit(OpCodes.Call, typeof(ParameterConverter).GetMethod("ProtoToMessage"));
            //now stack has a RpcMessage.Parameter
        }
    }
}

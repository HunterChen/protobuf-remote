using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ProtoBufRemote
{
    internal static class ServiceThunkBuilder
    {
        public static Delegate BuildThunk(RpcMethodDescriptor method)
        {
            var dynMethod = new DynamicMethod("ServiceThunk", typeof(void),
                new[] { typeof(object), typeof(IList<RpcMessage.Parameter>), typeof(RpcMessage.Result) },
                typeof(ServiceThunkBuilder).Module);

            ILGenerator ilGen = dynMethod.GetILGenerator();

            //verify number of parameters
            ilGen.Emit(OpCodes.Ldarg_1); //push IList<RpcMessage.Parameter>
            ilGen.Emit(OpCodes.Ldc_I4, method.ParameterTypes.Length); //push number of expected parameters
            ilGen.Emit(OpCodes.Ldarg_2); //push RpcMessage.Result
            ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("VerifyParameterCount"));
            Label paramVerifyLabel = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brtrue, paramVerifyLabel);
            ilGen.Emit(OpCodes.Ret);
            ilGen.MarkLabel(paramVerifyLabel);

            //load the RpcMessage.Result to the stack, for the call to set the result
            if (method.ReturnType != typeof(void))
                ilGen.Emit(OpCodes.Ldarg_2);
            //load the service instance to the stack, for the call to the service instance
            ilGen.Emit(OpCodes.Ldarg_0);

            //load the service call parameters to the stack, if they are the wrong type then an error is stored
            //in the result message and we return early
            MethodInfo paramIndexer = typeof(IList<RpcMessage.Parameter>).GetProperty("Item").GetGetMethod();
            for (int i = 0; i < method.ParameterTypes.Length; ++i )
            {
                Type paramType = method.ParameterTypes[i];

                //get RpcMessage.Parameter by calling IList indexer
                ilGen.Emit(OpCodes.Ldarg_1);
                ilGen.Emit(OpCodes.Ldc_I4, i);
                ilGen.Emit(OpCodes.Callvirt, paramIndexer);

                ilGen.Emit(OpCodes.Ldarg_2); //push RpcMessage.Result
                LocalBuilder paramLocal = ilGen.DeclareLocal(paramType);
                ilGen.Emit(OpCodes.Ldloca, (short)paramLocal.LocalIndex); //push ref to result local
                EmitParameter(ilGen, paramType);

                Label continueLabel = ilGen.DefineLabel();
                ilGen.Emit(OpCodes.Brtrue, continueLabel); //success, continue with next parameter

                //failed, stack cleanup
                for (int j = 0; j < i; ++j)
                    ilGen.Emit(OpCodes.Pop); //pop previous parameters
                ilGen.Emit(OpCodes.Pop); //pop service instance
                if (method.ReturnType != typeof(void))
                    ilGen.Emit(OpCodes.Pop); //pop RpcMessage.Result
                ilGen.Emit(OpCodes.Ret);

                ilGen.MarkLabel(continueLabel);

                //now push the actual parameter ready for the actual call
                ilGen.Emit(OpCodes.Ldloc, paramLocal.LocalIndex);
            }

            //now call the actual service instance method
            ilGen.Emit(OpCodes.Callvirt, method.SyncCallMethod);

            //get the return value, storing it in the result message. We know the type, no need for any error
            //checking
            if (method.ReturnType != typeof(void))
            {
                //check if result is null
                ilGen.Emit(OpCodes.Ldarg_2);
                ilGen.Emit(OpCodes.Ldnull);
                Label nullReturnLabel = ilGen.DefineLabel();
                ilGen.Emit(OpCodes.Beq, nullReturnLabel);

                //convert return value to a message and store it in the result message
                ThunkHelpers.EmitParameterToMessage(ilGen, method.ReturnType);
                ilGen.Emit(OpCodes.Call, typeof(RpcMessage.Result).GetProperty("CallResult").GetSetMethod());
                ilGen.Emit(OpCodes.Ret);

                ilGen.MarkLabel(nullReturnLabel);
                ilGen.Emit(OpCodes.Pop); //pop return value
                ilGen.Emit(OpCodes.Pop); //pop result message
            }

            ilGen.Emit(OpCodes.Ret);

            return dynMethod.CreateDelegate(typeof(RpcServer.MethodDelegate));
        }

        private static void EmitParameter(ILGenerator ilGen, Type paramType)
        {
            //stack already has a RpcMessage.Parameter, RpcMessage.Result, and ref to output local
            if (paramType == typeof(int) || paramType == typeof(sbyte) || paramType == typeof(short))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("IntFromMessage"));
            else if (paramType == typeof(uint) || paramType == typeof(byte)
                    || paramType == typeof(char) || paramType == typeof(ushort))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("UintFromMessage"));
            else if (paramType == typeof(long))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("Int64FromMessage"));
            else if (paramType == typeof(ulong))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("Uint64FromMessage"));
            else if (paramType == typeof(bool))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("BoolFromMessage"));
            else if (paramType == typeof(float))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("FloatFromMessage"));
            else if (paramType == typeof(double))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("DoubleFromMessage"));
            else if (paramType == typeof(string))
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("StringFromMessage"));
            else
            {
                ilGen.Emit(OpCodes.Ldtoken, paramType);
                ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                ilGen.Emit(OpCodes.Call, typeof(ServiceThunkHelpers).GetMethod("ProtoFromMessage"));
            }
            //stack now has true/false success result
        }
    }
}

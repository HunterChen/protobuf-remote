using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ProtoBufRemote
{
    internal class ProxyBuilder
    {
        private readonly ModuleBuilder moduleBuilder;
        private readonly MethodInfo blockingCallHelperMethodInfo;
        private readonly MethodInfo callWithoutResultHelperMethodInfo;
        private readonly MethodInfo beginAsyncCallHelperMethodInfo;
        private readonly MethodInfo endAsyncCallHelperMethodInfo;

        public ProxyBuilder()
        {
            var assemblyName = new AssemblyName("ProxyAssembly");
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName,
                AssemblyBuilderAccess.RunAndCollect);

            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            blockingCallHelperMethodInfo = typeof(Proxy).GetMethod("BlockingCallHelper",
                BindingFlags.Instance|BindingFlags.NonPublic, null,
                new[] { typeof(string), typeof(RpcMessage.Parameter[]) }, null);
            callWithoutResultHelperMethodInfo = typeof(Proxy).GetMethod("CallWithoutResultHelper",
                BindingFlags.Instance|BindingFlags.NonPublic, null,
                new[] { typeof(string), typeof(RpcMessage.Parameter[]) }, null);
            beginAsyncCallHelperMethodInfo = typeof(Proxy).GetMethod("BeginAsyncCallHelper",
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(string), typeof(RpcMessage.Parameter[]), typeof(AsyncCallback), typeof(object) }, null);
            endAsyncCallHelperMethodInfo = typeof(Proxy).GetMethod("EndAsyncCallHelper",
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(string), typeof(IAsyncResult) }, null);
        }

        public Type Build(Type serviceType)
        {
            if (!serviceType.IsInterface)
                throw new ArgumentException(String.Format(
                    "Type '{0}' being used to create a proxy must be an interface", serviceType.FullName));

            var service = new RpcServiceDescriptor(serviceType);

            TypeBuilder tb = moduleBuilder.DefineType(serviceType.Name + "Proxy", TypeAttributes.Public,
                typeof(Proxy), new[] { serviceType });

            ConstructorBuilder ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                new[] { typeof(RpcClient) });

            ILGenerator ctorIlGen = ctor.GetILGenerator();
            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Ldarg_1);
            ctorIlGen.Emit(OpCodes.Ldstr, service.Name);
            ctorIlGen.Emit(OpCodes.Call, typeof(Proxy).GetConstructor(new[] { typeof(RpcClient), typeof(string) }));
            ctorIlGen.Emit(OpCodes.Ret);

            foreach (var method in service.Methods)
            {
                MethodBuilder mb = tb.DefineMethod(method.SyncCallMethod.Name,
                    MethodAttributes.Public|MethodAttributes.Virtual, method.ReturnType, method.ParameterTypes);

                ILGenerator methodIlGen = mb.GetILGenerator();
                methodIlGen.Emit(OpCodes.Ldarg_0);
                methodIlGen.Emit(OpCodes.Ldstr, method.Name);
                EmitParameters(methodIlGen, method.ParameterTypes);
                if (method.ReturnType == typeof(void))
                {
                    methodIlGen.Emit(OpCodes.Call, callWithoutResultHelperMethodInfo);
                }
                else
                {
                    methodIlGen.Emit(OpCodes.Call, blockingCallHelperMethodInfo);
                    EmitReturn(methodIlGen, method.ReturnType);
                }
                methodIlGen.Emit(OpCodes.Ret);

                if (method.HasAsyncDeclarations)
                {
                    MethodBuilder mbBegin = tb.DefineMethod(method.AsyncBeginCallMethod.Name,
                        MethodAttributes.Public|MethodAttributes.Virtual, typeof(IAsyncResult),
                        method.ParameterTypes.Concat(new[] { typeof(AsyncCallback), typeof(object) }).ToArray());

                    ILGenerator beginIlGen = mbBegin.GetILGenerator();
                    beginIlGen.Emit(OpCodes.Ldarg_0);
                    beginIlGen.Emit(OpCodes.Ldstr, method.Name);
                    EmitParameters(beginIlGen, method.ParameterTypes);
                    beginIlGen.Emit(OpCodes.Ldarg, (short)(method.ParameterTypes.Length + 1)); //callback
                    beginIlGen.Emit(OpCodes.Ldarg, (short)(method.ParameterTypes.Length + 2)); //state
                    beginIlGen.Emit(OpCodes.Call, beginAsyncCallHelperMethodInfo);
                    beginIlGen.Emit(OpCodes.Ret);

                    MethodBuilder mbEnd = tb.DefineMethod(method.AsyncEndCallMethod.Name,
                        MethodAttributes.Public|MethodAttributes.Virtual,
                        method.ReturnType, new[] { typeof(IAsyncResult) });

                    ILGenerator endIlGen = mbEnd.GetILGenerator();
                    endIlGen.Emit(OpCodes.Ldarg_0);
                    endIlGen.Emit(OpCodes.Ldstr, method.Name);
                    endIlGen.Emit(OpCodes.Ldarg_1);
                    endIlGen.Emit(OpCodes.Call, endAsyncCallHelperMethodInfo);
                    if (method.ReturnType == typeof(void))
                        endIlGen.Emit(OpCodes.Pop);
                    else
                        EmitReturn(endIlGen, method.ReturnType);
                    endIlGen.Emit(OpCodes.Ret);
                }
            }

            return tb.CreateType();
        }

        private static void EmitParameters(ILGenerator ilGen, Type[] parameterTypes)
        {
            //Take the parameters passed as arguments and convert them to an array of RpcMessage.Parameters.
            if (parameterTypes.Length > 0)
            {
                ilGen.Emit(OpCodes.Ldc_I4, parameterTypes.Length);
                ilGen.Emit(OpCodes.Newarr, typeof(RpcMessage.Parameter));
                for (int i = 0; i < parameterTypes.Length; ++i)
                {
                    ilGen.Emit(OpCodes.Dup); //for array store
                    ilGen.Emit(OpCodes.Ldc_I4, i); //index for array store

                    ilGen.Emit(OpCodes.Ldarg, (short)i+1); //argument for ParameterConverter function

                    ThunkHelpers.EmitParameterToMessage(ilGen, parameterTypes[i]);

                    ilGen.Emit(OpCodes.Stelem_Ref); //array store result from ParameterConverter
                }
            }
            else
            {
                ilGen.Emit(OpCodes.Ldnull);
            }
        }

        private static void EmitReturn(ILGenerator ilGen, Type returnType)
        {
            //convert the RpcMessage.Parameter on the stack into a returnType and push onto the stack
            if (returnType == typeof(int) || returnType == typeof(sbyte) || returnType == typeof(short))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("IntFromMessage"));
            else if (returnType == typeof(uint) || returnType == typeof(byte)
                    || returnType == typeof(char) || returnType == typeof(ushort))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("UintFromMessage"));
            else if (returnType == typeof(long))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("Int64FromMessage"));
            else if (returnType == typeof(ulong))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("Uint64FromMessage"));
            else if (returnType == typeof(bool))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("BoolFromMessage"));
            else if (returnType == typeof(float))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("FloatFromMessage"));
            else if (returnType == typeof(double))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("DoubleFromMessage"));
            else if (returnType == typeof(string))
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("StringFromMessage"));
            else
            {
                ilGen.Emit(OpCodes.Ldtoken, returnType);
                ilGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                ilGen.Emit(OpCodes.Call, typeof(ProxyBuilderHelpers).GetMethod("ProtoFromMessage"));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace ProtoBufRemote
{
    internal class RpcMethodDescriptor
    {
        private readonly string name;
        private readonly string serviceName;
        private MethodInfo syncMethodInfo;
        private MethodInfo asyncBeginMethodInfo;
        private MethodInfo asyncEndMethodInfo;
        private Type[] parameterTypes;
        private Type returnType;

        public RpcMethodDescriptor(MethodInfo methodInfo, RpcMethodAttribute attr, string serviceName)
        {
            this.serviceName = serviceName;

            Type[] methodParamTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();

            if (methodInfo.Name.StartsWith("Begin") && methodParamTypes.Length >= 2
                && methodInfo.ReturnType == typeof(IAsyncResult)
                && methodParamTypes[methodParamTypes.Length - 2] == typeof(AsyncCallback)
                && methodParamTypes[methodParamTypes.Length - 1] == typeof(object))
            {
                asyncBeginMethodInfo = methodInfo;
                parameterTypes = methodParamTypes.Take(methodParamTypes.Length-2).ToArray();
                if (attr != null && attr.Name != null)
                    name = attr.Name;
                else
                    name = methodInfo.Name.Substring(5);
            }
            else if (methodInfo.Name.StartsWith("End") && methodParamTypes.Length == 1
                && methodParamTypes[0] == typeof(IAsyncResult))
            {
                asyncEndMethodInfo = methodInfo;
                returnType = methodInfo.ReturnType;
                if (attr != null && attr.Name != null)
                    name = attr.Name;
                else
                    name = methodInfo.Name.Substring(3);
            }
            else
            {
                syncMethodInfo = methodInfo;
                parameterTypes = methodParamTypes;
                returnType = methodInfo.ReturnType;
                if (attr != null && attr.Name != null)
                    name = attr.Name;
                else
                    name = methodInfo.Name;
            }

            //verify parameters
            if (parameterTypes != null)
            {
                foreach (Type paramType in parameterTypes)
                {
                    if (!ParameterConverter.IsSupportedType(paramType))
                        throw new ArgumentException(
                            String.Format(
                                "Parameter of type '{0}' on method '{1}' in service '{2}' is not a supported type.",
                                paramType, methodInfo.Name, serviceName));
                }
            }

            //verify return type
            if (returnType != null && returnType != typeof(void))
            {
                if (!ParameterConverter.IsSupportedType(returnType))
                    throw new ArgumentException(
                        String.Format("Return type '{0}' of method '{1}' in service '{2}' is not a supported type",
                            returnType, name, serviceName));
            }
        }

        public string Name { get { return name; } }

        public bool HasAsyncDeclarations { get { return (asyncBeginMethodInfo != null); } }

        public MethodInfo SyncCallMethod { get { return syncMethodInfo; } }

        public MethodInfo AsyncBeginCallMethod { get { return asyncBeginMethodInfo; } }

        public MethodInfo AsyncEndCallMethod { get { return asyncEndMethodInfo; } }

        public Type[] ParameterTypes { get { return parameterTypes; } }

        public Type ReturnType { get { return returnType; } }

        public void Merge(RpcMethodDescriptor method)
        {
            Debug.Assert(method.name == name && method.serviceName == serviceName);

            if (returnType != null && method.returnType != null && returnType != method.returnType)
                throw new ArgumentException(
                    String.Format("Invalid declarations for method '{0}', the return types are not consistent.", name));

            if (parameterTypes != null && method.parameterTypes != null
                && !parameterTypes.SequenceEqual(method.parameterTypes))
                throw new ArgumentException(
                    String.Format("Invalid declarations for method '{0}', the parameter types are not consistent.", name));

            if (syncMethodInfo != null && method.syncMethodInfo != null)
                throw new ArgumentException(
                    String.Format("Invalid declarations for method '{0}', more than one synchronous call found.", name));
            if (asyncBeginMethodInfo != null && method.asyncBeginMethodInfo != null)
                throw new ArgumentException(
                    String.Format("Invalid declarations for method '{0}', more than one begin async call found.", name));
            if (asyncEndMethodInfo != null && method.asyncEndMethodInfo != null)
                throw new ArgumentException(
                    String.Format("Invalid declarations for method '{0}', more than one end async call found.", name));

            returnType = returnType ?? method.returnType;
            parameterTypes = parameterTypes ?? method.parameterTypes;
            syncMethodInfo = syncMethodInfo ?? method.syncMethodInfo;
            asyncBeginMethodInfo = asyncBeginMethodInfo ?? method.asyncBeginMethodInfo;
            asyncEndMethodInfo = asyncEndMethodInfo ?? method.asyncEndMethodInfo;
        }

        public bool IsAsyncDeclarationValid()
        {
            if (asyncBeginMethodInfo != null && asyncEndMethodInfo != null)
                return true;
            if (asyncBeginMethodInfo == null && asyncEndMethodInfo == null)
                return true;
            return false;
        }

        public void Invoke(object service, IList<RpcMessage.Parameter> parameters, RpcMessage resultMessage)
        {
            ParameterInfo[] parameterInfos = syncMethodInfo.GetParameters();
            if (parameterInfos.Length != parameters.Count)
            {
                if (resultMessage != null)
                {
                    resultMessage.ResultMessage.IsFailed = true;
                    resultMessage.ResultMessage.ErrorMessage = String.Format(
                        "Wrong number of parameters for method '{1}' in service '{0}'", serviceName, Name);
                }
                return;
            }

            var invokeParameters = new object[parameters.Count];
            for (int i=0; i<parameters.Count; ++i)
            {
                string errorMsg;
                if (!ParameterConverter.FromMessage(parameters[i], parameterInfos[i].ParameterType,
                    out invokeParameters[i], out errorMsg))
                {
                    if (resultMessage != null)
                    {
                        resultMessage.ResultMessage.IsFailed = true;
                        resultMessage.ResultMessage.ErrorMessage = String.Format(
                            "Parameter {0} could not be converted to expected type '{1}' for method '{2}' in service '{3}' ({4}).",
                            i, parameterInfos[i].ParameterType, Name, serviceName, errorMsg);
                    }
                    return;
                }
            }

            object result = syncMethodInfo.Invoke(service, invokeParameters);

            if (resultMessage != null)
            {
                RpcMessage.Parameter resultParameter;
                string errorMsg;
                if (ParameterConverter.ToMessage(result, out resultParameter, out errorMsg))
                {
                    resultMessage.ResultMessage.CallResult = resultParameter;
                }
                else
                {
                    resultMessage.ResultMessage.IsFailed = true;
                    resultMessage.ResultMessage.ErrorMessage = String.Format(
                        "Result type {0} could not be converted to a message for method '{1}' in service '{2}' ({3}).",
                        syncMethodInfo.ReturnType, Name, serviceName, errorMsg);
                    return;                    
                }
            }
        }
    }
}

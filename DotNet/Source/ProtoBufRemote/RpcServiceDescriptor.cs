using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProtoBufRemote
{
    /// <summary>
    /// Builds a description of an interface or class used as a service, parsing the attributes.
    /// </summary>
    internal class RpcServiceDescriptor
    {
        private readonly string name;
        private readonly IDictionary<string, RpcMethodDescriptor> methods =
            new Dictionary<string, RpcMethodDescriptor>();

        /// <summary>
        /// The name of the RPC service
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// The RPC methods implemented by the service.
        /// </summary>
        public IEnumerable<RpcMethodDescriptor> Methods { get { return methods.Values; } }

        /// <summary>
        /// Builds a descriptor of the service based on the type, collects the methods and parses attributes.
        /// </summary>
        /// <param name="serviceType"></param>
        public RpcServiceDescriptor(Type serviceType)
        {
            Type typeWithServiceAttr = null;
            RpcServiceAttribute serviceAttr = GetServiceAttribute(serviceType, ref typeWithServiceAttr);
            if (serviceAttr == null)
            {
                if (!serviceType.IsInterface)
                    throw new ArgumentException(
                        "Only interface types can be used as services without explicit attributes");
                name = serviceType.Name;
                InitMethodsFromType(serviceType, false);
            }
            else
            {
                if (!typeWithServiceAttr.IsInterface && !serviceAttr.IsAttributedMethodsOnly)
                    throw new ArgumentException(
                        "Only interface types can be used as services without explicit attributes");
                name = serviceAttr.Name ?? typeWithServiceAttr.Name;
                InitMethodsFromType(typeWithServiceAttr, serviceAttr.IsAttributedMethodsOnly);
            }

            if (methods.Count == 0)
                throw new ArgumentException("Service parameter does not have any RPC methods");

            foreach (var method in Methods)
            {
                if (!method.IsAsyncDeclarationValid())
                    throw new ArgumentException(String.Format(
                        "Invalid declarations for method '{0}', both or neither begin and end async methods must be declared",
                        method.Name));
            }
        }

        public RpcMethodDescriptor GetMethod(string methodName)
        {
            RpcMethodDescriptor method;
            if (methods.TryGetValue(methodName, out method))
                return method;
            return null;
        }

        public void InvokeMethod(object service, string methodName, List<RpcMessage.Parameter> parameters, RpcMessage resultMessage)
        {
            RpcMethodDescriptor method = GetMethod(methodName);
            if (method != null)
            {
                method.Invoke(service, parameters, resultMessage);
            }
            else
            {
                //return an error message to the client if it is expecting a result, otherwise we fail silently
                if (resultMessage != null)
                {
                    resultMessage.ResultMessage.IsFailed = true;
                    resultMessage.ResultMessage.ErrorMessage = String.Format("Unknown method '{1}' in service '{0}'",
                        Name, methodName);
                }
            }
        }

        private void InitMethodsFromType(Type type, bool isAttributedMethodsOnly)
        {
            foreach (var methodInfo in type.GetMethods())
            {
                RpcMethodAttribute methodAttr = GetMethodAttribute(methodInfo);
                if (methodAttr != null || !isAttributedMethodsOnly)
                {
                    RpcMethodDescriptor method = new RpcMethodDescriptor(methodInfo, methodAttr, name);
                    if (methods.ContainsKey(method.Name))
                    {
                        methods[method.Name].Merge(method);
                    }
                    else
                    {
                        methods.Add(method.Name, method);
                    }
                }
            }

            //for classes checking the public methods is enough, but for an interface we must check the other
            //interfaces implemented too
            if (type.IsInterface)
            {
                foreach (var interfaceType in type.GetInterfaces())
                    InitMethodsFromType(interfaceType, isAttributedMethodsOnly);
            }
        }
        
        private static RpcServiceAttribute GetServiceAttribute(Type serviceType, ref Type typeWithAttr)
        {
            Type type = serviceType;
            while (type != null)
            {
                object[] serviceAttrs = type.GetCustomAttributes(typeof(RpcServiceAttribute), false);
                if (serviceAttrs.Length > 0)
                {
                    typeWithAttr = type;
                    return (RpcServiceAttribute)serviceAttrs[0];
                }
                type = type.BaseType;
            }

            Type[] interfaces = serviceType.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                object[] serviceAttrs = interfaceType.GetCustomAttributes(typeof(RpcServiceAttribute), true);
                if (serviceAttrs.Length > 0)
                {
                    typeWithAttr = interfaceType;
                    return (RpcServiceAttribute)serviceAttrs[0];
                }
            }

            return null;
        }

        private static RpcMethodAttribute GetMethodAttribute(MethodInfo methodInfo)
        {
            object[] attrs = methodInfo.GetCustomAttributes(typeof(RpcMethodAttribute), true);
            if (attrs.Length > 0)
                return (RpcMethodAttribute)attrs[0];
            return null;
        }
    }
}

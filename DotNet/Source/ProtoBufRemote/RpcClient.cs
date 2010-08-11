using System;
using System.Collections.Generic;

namespace ProtoBufRemote
{
    public class RpcClient
    {
        private int nextId;
        private readonly ProxyBuilder proxyBuilder = new ProxyBuilder();
        private readonly IDictionary<int, PendingCall> pendingCalls = new Dictionary<int, PendingCall>();
        private readonly IDictionary<Type, Type> proxyTypes = new Dictionary<Type, Type>();

        private readonly RpcController controller;

        public RpcClient(RpcController controller)
        {
            this.controller = controller;
            controller.Client = this;
        }

        public TService GetProxy<TService>() where TService : class
        {
            Type proxyType;
            if (!proxyTypes.TryGetValue(typeof(TService), out proxyType))
            {
                proxyType = proxyBuilder.Build(typeof(TService));
                proxyTypes.Add(typeof(TService), proxyType);
            }
            return (TService)Activator.CreateInstance(proxyType, this);
        }

        public dynamic GetProxy(string serviceName)
        {
            return new DynamicProxy(this, serviceName);
        }

        internal virtual PendingCall Call(string serviceName, string methodName, object[] args, AsyncCallback callback,
                                          object state)
        {
            RpcMessage.Parameter[] parameters = CreateParameters(args, serviceName, methodName);
            return Call(serviceName, methodName, parameters, callback, state);
        }

        internal virtual PendingCall Call(string serviceName, string methodName, RpcMessage.Parameter[] parameters,
                                          AsyncCallback callback, object state)
        {
            var message = new RpcMessage();
            message.CallMessage = new RpcMessage.Call();
            message.CallMessage.Service = serviceName;
            message.CallMessage.Method = methodName;
            message.CallMessage.ExpectsResult = true;
            if (parameters != null)
            {
                foreach (var param in parameters)
                    message.CallMessage.Parameters.Add(param);
            }

            PendingCall pendingCall;

            lock (pendingCalls)
            {
                int id = GetFreeId();
                message.Id = id;
                pendingCall = new PendingCall(id, serviceName, methodName, callback, state);
                pendingCalls.Add(id, pendingCall);
            }

            //channel send is thread-safe already
            controller.Send(message);

            return pendingCall;
        }

        internal virtual void CallWithoutResult(string serviceName, string methodName, object[] args)
        {
            CallWithoutResult(serviceName, methodName, CreateParameters(args, serviceName, methodName));
        }

        internal virtual void CallWithoutResult(string serviceName, string methodName,
                                                RpcMessage.Parameter[] parameters)
        {
            var message = new RpcMessage();
            message.CallMessage = new RpcMessage.Call();
            message.CallMessage.Service = serviceName;
            message.CallMessage.Method = methodName;
            message.CallMessage.ExpectsResult = false;
            if (parameters != null)
            {
                foreach (var param in parameters)
                    message.CallMessage.Parameters.Add(param);
            }

            //channel send is thread-safe already
            controller.Send(message);
        }

        internal virtual void ReceiveResult(RpcMessage message)
        {
            PendingCall pendingCall = pendingCalls[message.Id];
            pendingCalls.Remove(message.Id);
            pendingCall.ReceiveResult(message.ResultMessage);
        }

        private int GetFreeId()
        {
            return nextId++;
        }

        private static RpcMessage.Parameter[] CreateParameters(object[] args, string service, string method)
        {
            RpcMessage.Parameter[] parameters = new RpcMessage.Parameter[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                string errorMsg;
                if (!ParameterConverter.ToMessage(args[i], out parameters[i], out errorMsg))
                {
                    throw new InvalidRpcCallException(service, method,
                        String.Format("Couldn't use parameter in rpc call ({0}).", errorMsg));

                }
            }
            return parameters;
        }
    }
}

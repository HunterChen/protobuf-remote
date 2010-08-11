using System;
using System.Collections.Generic;

namespace ProtoBufRemote
{
    /// <summary>
    /// Handles received calls from a channel by forwarding them to the registered services, and returning the result.
    /// </summary>
    public class RpcServer
    {
        private readonly RpcController controller;
        private readonly IDictionary<string, ServiceInstance> services = new Dictionary<string, ServiceInstance>();

        public RpcServer(RpcController controller)
        {
            this.controller = controller;
            controller.Server = this;
        }

        /// <summary>
        /// Registers a service implementation with the RpcServer, it will be used to process received calls.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service can be specified explicitly, this allows the name of the service to be discovered
        /// even when the type does not have attributes.
        /// </typeparam>
        /// <param name="service"></param>
        public void RegisterService<TService>(TService service)
        {
            var rpcService = new RpcServiceDescriptor(typeof(TService));

            if (services.ContainsKey(rpcService.Name))
                throw new ArgumentException(String.Format("A service with name '{0}' has already been registered",
                    rpcService.Name));

            var serviceInstance = new ServiceInstance { Descriptor = rpcService, Instance = service };
            foreach (var method in rpcService.Methods)
            {
                serviceInstance.MethodThunks.Add(method.Name, (MethodDelegate)ServiceThunkBuilder.BuildThunk(method));
            }

            services.Add(rpcService.Name, serviceInstance);
        }

        internal virtual void ReceiveCall(RpcMessage message)
        {
            RpcMessage resultMessage = null;
            if (message.CallMessage.ExpectsResult)
            {
                resultMessage = new RpcMessage();
                resultMessage.Id = message.Id;
                resultMessage.ResultMessage = new RpcMessage.Result();
            }

            ServiceInstance service;
            if (services.TryGetValue(message.CallMessage.Service, out service))
            {
                MethodDelegate thunk;
                if (service.MethodThunks.TryGetValue(message.CallMessage.Method, out thunk))
                {
                    thunk(service.Instance, message.CallMessage.Parameters,
                        resultMessage != null ? resultMessage.ResultMessage : null);
                }
                else
                {
                    if (resultMessage != null)
                    {
                        resultMessage.ResultMessage.IsFailed = true;
                        resultMessage.ResultMessage.ErrorMessage = String.Format(
                            "Unknown method '{0}' in service '{1}'",
                            message.CallMessage.Method, message.CallMessage.Service);
                    }
                }
            }
            else
            {
                if (resultMessage != null)
                {
                    resultMessage.ResultMessage.IsFailed = true;
                    resultMessage.ResultMessage.ErrorMessage = String.Format("Unknown service '{0}'",
                        message.CallMessage.Service);
                }
            }

            if (resultMessage != null)
                controller.Send(resultMessage);
        }

        internal delegate void MethodDelegate(object serviceInstance, IList<RpcMessage.Parameter> parameters,
                                             RpcMessage.Result resultMessage);

        internal class ServiceInstance
        {
            private readonly IDictionary<string, MethodDelegate> methodThunks = new Dictionary<string, MethodDelegate>();

            public object Instance { get; set; }
            public RpcServiceDescriptor Descriptor { get; set; }
            public IDictionary<string, MethodDelegate> MethodThunks { get { return methodThunks; } }
        }
    }
}

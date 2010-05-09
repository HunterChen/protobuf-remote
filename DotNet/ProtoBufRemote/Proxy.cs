using System;

namespace ProtoBufRemote
{
    public class Proxy
    {
        private readonly RpcClient client;
        private readonly string serviceName;

        public Proxy(RpcClient client, string serviceName)
        {
            this.client = client;
            this.serviceName = serviceName;
        }

        protected void CallWithoutResultHelper(string methodName, RpcMessage.Parameter[] parameters)
        {
            client.CallWithoutResult(serviceName, methodName, parameters);
        }

        protected RpcMessage.Parameter BlockingCallHelper(string methodName, RpcMessage.Parameter[] parameters)
        {
            IAsyncResult asyncResult = BeginAsyncCallHelper(methodName, parameters, null, null);
            return EndAsyncCallHelper(methodName, asyncResult);
        }

        protected IAsyncResult BeginAsyncCallHelper(string methodName, RpcMessage.Parameter[] parameters,
                                                    AsyncCallback callback, object state)
        {
            return client.Call(serviceName, methodName, parameters, callback, state);
        }

        protected RpcMessage.Parameter EndAsyncCallHelper(string methodName, IAsyncResult asyncResult)
        {
            PendingCall pendingCall = (PendingCall)asyncResult;

            pendingCall.AsyncWaitHandle.WaitOne();
            pendingCall.AsyncWaitHandle.Close();

            if (pendingCall.IsFailed)
                throw new InvalidRpcCallException(serviceName, methodName,
                    String.Format("Server failed to process call, returned error message: \"{0}\".",
                    pendingCall.ServerErrorMessage));

            return pendingCall.Result;
        }
    }
}

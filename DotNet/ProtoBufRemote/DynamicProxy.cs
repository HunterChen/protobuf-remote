using System;
using System.Dynamic;

namespace ProtoBufRemote
{
    internal class DynamicProxy : DynamicObject
    {
        private readonly RpcClient client;
        private readonly string serviceName;

        public DynamicProxy(RpcClient client, string serviceName)
        {
            this.client = client;
            this.serviceName = serviceName;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name.StartsWith("Begin") && args.Length >= 2)
            {
                //if the 2nd last argument is either null or convertible to an AsyncCallback, then we decide this is
                //an async call
                object callbackArg = args[args.Length-2];
                AsyncCallback callback = callbackArg as AsyncCallback;
                if (callbackArg == null || callback != null)
                {
                    string methodName = binder.Name.Substring(5);
                    object[] newArgs = new object[args.Length-2];
                    Array.Copy(args, newArgs, args.Length - 2);
                    object state = args[args.Length - 1];
                    result = client.Call(serviceName, methodName, newArgs, callback, state);
                    return true;
                }
            }

            PendingCall pendingCall;
            if (binder.Name.StartsWith("End") && args.Length == 1 && args[0] is IAsyncResult)
                pendingCall = (PendingCall)args[0];
            else
                pendingCall = client.Call(serviceName, binder.Name, args, null, null);

            //We always block for a response on dynamic calls, because there's no way to tell if the user is expecting
            //a return type. Attempting to block in the DynamicCallResult is possible, but makes it difficult to clean
            //up the PendingCall as the TryConvert may never be called.
            pendingCall.AsyncWaitHandle.WaitOne();
            pendingCall.AsyncWaitHandle.Close();

            if (pendingCall.IsFailed)
                throw new InvalidRpcCallException(serviceName, binder.Name,
                    String.Format("Server failed to process call, returned error message: \"{0}\".",
                    pendingCall.ServerErrorMessage));
            result = new DynamicCallResult(pendingCall);
            return true;
        }
    }
}

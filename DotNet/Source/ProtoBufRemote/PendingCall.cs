using System;
using System.Threading;

namespace ProtoBufRemote
{
    internal class PendingCall : IAsyncResult
    {
        private readonly int id;
        private readonly string serviceName;
        private readonly string methodName;
        private readonly AsyncCallback asyncCallback;
        private readonly object asyncState;
        private bool isCompleted;
        private bool isFailed;
        private string serverErrorMessage;
        private readonly ManualResetEvent waitHandle = new ManualResetEvent(false);
        private RpcMessage.Parameter result;

        public PendingCall(int id, string serviceName, string methodName, AsyncCallback asyncCallback,
                           object asyncState)
        {
            this.id = id;
            this.serviceName = serviceName;
            this.methodName = methodName;
            this.asyncCallback = asyncCallback;
            this.asyncState = asyncState;
        }

        public int Id { get { return id; } }

        public bool IsFailed { get { return isFailed; } }

        public string ServerErrorMessage { get { return serverErrorMessage; } }

        public object AsyncState { get { return asyncState; } }

        public bool IsCompleted { get { return isCompleted; } }

        public bool CompletedSynchronously { get { return false; } }

        public WaitHandle AsyncWaitHandle { get { return waitHandle; } }

        public RpcMessage.Parameter Result { get { return result; } }

        public object GetResultAs(Type type)
        {
            //we shouldn't be trying to access the return type if the server returned nothing
            if (result == null)
                throw new InvalidRpcCallException(serviceName, methodName,
                    String.Format("Server had no return type, was expecting a return type {0}.", type));

            object param;
            string errorMsg;
            if (!ParameterConverter.FromMessage(result, type, out param, out errorMsg))
            {
                throw new InvalidRpcCallException(serviceName, methodName,
                    String.Format("Failed to convert the rpc return value to the expected return type {0} ({1}).",
                    type, errorMsg));
                
            }
            return param;
        }

        public void ReceiveResult(RpcMessage.Result resultMessage)
        {
            result = resultMessage.CallResult;
            isCompleted = true;
            isFailed = resultMessage.IsFailed;
            serverErrorMessage = resultMessage.ErrorMessage;
            if (asyncCallback != null)
                asyncCallback(this);
            waitHandle.Set();
        }
    }
}
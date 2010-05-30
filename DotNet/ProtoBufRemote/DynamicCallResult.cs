using System.Dynamic;

namespace ProtoBufRemote
{
    internal class DynamicCallResult : DynamicObject
    {
        private readonly PendingCall pendingCall;

        public DynamicCallResult(PendingCall pendingCall)
        {
            this.pendingCall = pendingCall;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = pendingCall.GetResultAs(binder.Type);
            return true;
        }
    }
}

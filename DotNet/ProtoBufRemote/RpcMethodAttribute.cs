using System;

namespace ProtoBufRemote
{
    /// <summary>
    /// Optional attribute for a method in a service class. This is used to mark the RPC methods if
    /// RpcServiceAttribute.IsAttributedMethodsOnly is true, or to override the method name.
    /// </summary>
    public class RpcMethodAttribute : Attribute
    {
        /// <summary>
        /// Overrides the name of the RPC method used when making the actual RPC call. By default this is the name of
        /// the method itself.
        /// </summary>
        public string Name { get; set; }
    }
}

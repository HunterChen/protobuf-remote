using System;

namespace ProtoBufRemote
{
    /// <summary>
    /// Optional attribute which can be applied to an RPC service interface or class. Without this attribute the class
    /// name itself will be used as the service name and all public methods will be used as RPC methods.
    /// </summary>
    public class RpcServiceAttribute : Attribute
    {
        /// <summary>
        /// Overrides the name of the service, by default it will be same name as the interface/class.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Use only methods explicitly marked with an RpcMethodAttribute as RPC methods.
        /// </summary>
        public bool IsAttributedMethodsOnly { get; set; }
    }
}

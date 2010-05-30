using System;

namespace ProtoBufRemote
{
    /// <summary>
    /// Helper functions for converting parameters to and from protobuf messages. Only public because Reflection Emit
    /// generated proxies are technically in a different assembly.
    /// </summary>
    public static class ProxyBuilderHelpers
    {
        public static int IntFromMessage(RpcMessage.Parameter parameter)
        {
            int value = 0;
            string error = null;
            if (!ParameterConverter.IntFromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected int return type ({0})", error));
            return value;
        }

        public static uint UintFromMessage(RpcMessage.Parameter parameter)
        {
            uint value = 0;
            string error = null;
            if (!ParameterConverter.UintFromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected uint return type ({0})", error));
            return value;
        }

        public static long Int64FromMessage(RpcMessage.Parameter parameter)
        {
            long value = 0;
            string error = null;
            if (!ParameterConverter.Int64FromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected int64 return type ({0})", error));
            return value;
        }

        public static ulong Uint64FromMessage(RpcMessage.Parameter parameter)
        {
            ulong value = 0;
            string error = null;
            if (!ParameterConverter.Uint64FromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected uint64 return type ({0})", error));
            return value;
        }

        public static bool BoolFromMessage(RpcMessage.Parameter parameter)
        {
            bool value = false;
            string error = null;
            if (!ParameterConverter.BoolFromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected bool return type ({0})", error));
            return value;
        }

        public static float FloatFromMessage(RpcMessage.Parameter parameter)
        {
            float value = 0.0f;
            string error = null;
            if (!ParameterConverter.FloatFromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected float return type ({0})", error));
            return value;
        }

        public static double DoubleFromMessage(RpcMessage.Parameter parameter)
        {
            double value = 0.0;
            string error = null;
            if (!ParameterConverter.DoubleFromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected double return type ({0})", error));
            return value;
        }

        public static string StringFromMessage(RpcMessage.Parameter parameter)
        {
            string value = null;
            string error = null;
            if (!ParameterConverter.StringFromMessage(parameter, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected string return type ({0})", error));
            return value;
        }

        public static object ProtoFromMessage(RpcMessage.Parameter parameter, Type type)
        {
            object value = null;
            string error = null;
            if (!ParameterConverter.ProtoFromMessage(parameter, type, ref value, ref error))
                throw new ArgumentException(
                    String.Format("Cannot convert server return value into expected protobuf return type '{0}' ({1})",
                    type, error));
            return value;
        }
    }
}

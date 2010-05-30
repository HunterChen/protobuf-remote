using System;
using System.Collections.Generic;

namespace ProtoBufRemote
{
    internal static class ServiceThunkHelpers
    {
        public static bool VerifyParameterCount(IList<RpcMessage.Parameter> parameters, int expectedCount,
                                                RpcMessage.Result resultMessage)
        {
            if (parameters.Count != expectedCount)
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage = "Wrong number of parameters";
                }
                return false;
            }
            return true;
        }

        public static bool IntFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                          ref int value)
        {
            string error = null;
            if (!ParameterConverter.IntFromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected int type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool UintFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                           ref uint value)
        {
            string error = null;
            if (!ParameterConverter.UintFromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected uint type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool Int64FromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                            ref long value)
        {
            string error = null;
            if (!ParameterConverter.Int64FromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected long type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool Uint64FromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                             ref ulong value)
        {
            string error = null;
            if (!ParameterConverter.Uint64FromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected ulong type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool BoolFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                           ref bool value)
        {
            string error = null;
            if (!ParameterConverter.BoolFromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected bool type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool FloatFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                            ref float value)
        {
            string error = null;
            if (!ParameterConverter.FloatFromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected float type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool DoubleFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                             ref double value)
        {
            string error = null;
            if (!ParameterConverter.DoubleFromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected double type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool StringFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                             ref string value)
        {
            string error = null;
            if (!ParameterConverter.StringFromMessage(parameter, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected int type ({0})", error);
                }
                return false;
            }
            return true;
        }

        public static bool ProtoFromMessage(RpcMessage.Parameter parameter, RpcMessage.Result resultMessage,
                                            ref object value, Type paramType)
        {
            string error = null;
            if (!ParameterConverter.ProtoFromMessage(parameter, paramType, ref value, ref error))
            {
                if (resultMessage != null)
                {
                    resultMessage.IsFailed = true;
                    resultMessage.ErrorMessage =
                        String.Format("Cannot convert call parameter value into expected protobuf type ({0})", error);
                }
                return false;
            }
            return true;
        }
    }
}

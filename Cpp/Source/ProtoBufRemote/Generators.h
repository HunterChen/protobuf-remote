#ifndef PROTOBUFREMOTE_GENERATORS_H
#define PROTOBUFREMOTE_GENERATORS_H 1

#include <boost/preprocessor.hpp>
#include "ProtoBufRemote/PendingCall.h"
#include "ProtoBufRemote/Proxy.h"
#include "ProtoBufRemote/RpcClient.h"
#include "ProtoBufRemote/RpcService.h"

/**
 * Declares both a proxy and a service stub. Here is a sample declaration, note that there are no commas between methods
 * or between parameters.
 * PBR_SERVICE(SampleService2,
 *   PBR_METHOD(GetSquare, PBR_INT(PBR_INT))
 *   PBR_METHOD(DoStuff, PBR_VOID(PBR_STRING))
 *   PBR_METHOD(DoOtherStuff, PBR_VOID(PBR_STRING PBR_INT PBR_BOOL))
 *   PBR_METHOD(DoEvenMoreStuff, PBR_VOID(PBR_VOID))
 * )
 */
#define PBR_SERVICE(name, methods) \
	PBR_PROXY(name, methods) \
	PBR_SERVICE_STUB(name, methods)

/**
 * Declares a proxy only, without a matching service stub. See PBR_SERVICE for a description.
 */
#define PBR_PROXY(name, methods) \
	class name##Proxy : public ::ProtoBufRemote::Proxy \
	{ \
	public: \
		name##Proxy(::ProtoBufRemote::RpcClient* client) : ::ProtoBufRemote::Proxy(client, #name) { } \
		BOOST_PP_SEQ_FOR_EACH(PBR_X_PROXY_METHOD, ~, methods) \
	};

/**
 * Declares a service stub only, without a matching proxy. See PBR_SERVICE for a description.
 */
#define PBR_SERVICE_STUB(name, methods) \
	class name##Stub : public ::ProtoBufRemote::RpcService \
	{ \
	public: \
		name##Stub() : ::ProtoBufRemote::RpcService(#name) \
		{ \
		} \
		virtual bool Call(const char* methodName, const ::ProtoBufRemote::ParameterList& parameters, \
		                  ::ProtoBufRemote::MutableParameter* result) \
		{ \
			if (0) { } \
			BOOST_PP_SEQ_FOR_EACH(PBR_X_STUB_CALL_METHOD, ~, methods) \
			return false; \
		} \
		BOOST_PP_SEQ_FOR_EACH(PBR_X_STUB_METHOD, ~, methods) \
	};

#define PBR_METHOD(name, signature) ((name, signature))

//is non-void, is result as parameter ptr, can get ref, type, parameter type, return type, set func, get func, is set func
#define PBR_VOID ((0, 0, 0, void, void, void, ~, ~, ~))
#define PBR_CHAR ((1, 0, 1, signed char, signed char, signed char, SetChar, GetChar, IsChar))
#define PBR_UCHAR ((1, 0, 1, unsigned char, unsigned char, unsigned char, SetUnsignedChar, GetUnsignedChar, IsUnsignedChar))
#define PBR_SHORT ((1, 0, 1, short, short, short, SetShort, GetShort, IsShort))
#define PBR_USHORT ((1, 0, 1, unsigned short, unsigned short, unsigned short, SetUnsignedShort, GetUnsignedShort, IsUnsignedShort))
#define PBR_INT ((1, 0, 1, int, int, int, SetInt, GetInt, IsInt))
#define PBR_UINT ((1, 0, 1, unsigned int, unsigned int, unsigned int, SetUnsignedInt, GetUnsignedInt, IsUnsignedInt))
#define PBR_INT64 ((1, 0, 1, long long, long long, long long, SetInt64, GetInt64, IsInt64))
#define PBR_UINT64 ((1, 0, 1, unsigned long long, unsigned long long, unsigned long long, SetUnsignedInt64, GetUnsignedInt64, IsUnsignedInt64))
#define PBR_BOOL ((1, 0, 1, bool, bool, bool, SetBool, GetBool, IsBool))
#define PBR_STRING ((1, 1, 1, std::string, const std::string&, void, SetString, GetString, IsString))
#define PBR_WCHAR ((1, 0, 1, wchar_t, wchar_t, wchar_t, SetWChar, GetWChar, IsWChar))
#define PBR_PROTO(protoType) ((1, 1, 0, protoType, const protoType&, void, SetProto, GetProto, IsProto))


//=============================================================================================================
// internal macros
//=============================================================================================================

#define PBR_X_PARAM_IS_NON_VOID(param)   BOOST_PP_TUPLE_ELEM(9, 0, param)
#define PBR_X_PARAM_IS_RESULT_PTR(param) BOOST_PP_TUPLE_ELEM(9, 1, param)
#define PBR_X_PARAM_CAN_GET_REF(param)   BOOST_PP_TUPLE_ELEM(9, 2, param)
#define PBR_X_PARAM_TYPE(param)          BOOST_PP_TUPLE_ELEM(9, 3, param)
#define PBR_X_PARAM_PARAM_TYPE(param)    BOOST_PP_TUPLE_ELEM(9, 4, param)
#define PBR_X_PARAM_RETURN_TYPE(param)   BOOST_PP_TUPLE_ELEM(9, 5, param)
#define PBR_X_PARAM_SET_FUNC(param)      BOOST_PP_TUPLE_ELEM(9, 6, param)
#define PBR_X_PARAM_GET_FUNC(param)      BOOST_PP_TUPLE_ELEM(9, 7, param)
#define PBR_X_PARAM_CHECK_FUNC(param)    BOOST_PP_TUPLE_ELEM(9, 8, param)

#define PBR_X_METHOD_NAME(method) BOOST_PP_TUPLE_ELEM(2, 0, method)
#define PBR_X_METHOD_SIG(method) BOOST_PP_TUPLE_ELEM(2, 1, method)
#define PBR_X_METHOD_RETURN(method) BOOST_PP_SEQ_ELEM(0, PBR_X_METHOD_SIG(method))
#define PBR_X_METHOD_PARAMS(method) BOOST_PP_SEQ_ELEM(1, PBR_X_METHOD_SIG(method))
#define PBR_X_METHOD_HAS_RETURN(method) PBR_X_PARAM_IS_NON_VOID(PBR_X_METHOD_RETURN(method))
#define PBR_X_METHOD_HAS_PARAMS(method) PBR_X_PARAM_IS_NON_VOID(BOOST_PP_SEQ_HEAD(PBR_X_METHOD_PARAMS(method)))

#define PBR_X_DECLARE_METHOD_PARAMS(method) \
	BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_DECLARE_METHOD_PARAM, ~, PBR_X_METHOD_PARAMS(method))) \
	BOOST_PP_EXPR_IF( \
		BOOST_PP_AND(PBR_X_METHOD_HAS_PARAMS(method), PBR_X_PARAM_IS_RESULT_PTR(PBR_X_METHOD_RETURN(method))), \
		BOOST_PP_COMMA()) \
	BOOST_PP_EXPR_IF(PBR_X_PARAM_IS_RESULT_PTR(PBR_X_METHOD_RETURN(method)), \
		PBR_X_PARAM_TYPE(PBR_X_METHOD_RETURN(method))* resultPtr)

#define PBR_X_DECLARE_METHOD_PARAM(r, data, i, param) \
	BOOST_PP_COMMA_IF(i) PBR_X_PARAM_PARAM_TYPE(param) BOOST_PP_CAT(p, i)

//proxy

#define PBR_X_PROXY_METHOD(r, data, method) \
	PBR_X_PARAM_RETURN_TYPE(PBR_X_METHOD_RETURN(method)) PBR_X_METHOD_NAME(method)( \
		PBR_X_DECLARE_METHOD_PARAMS(method)) \
	{ \
		m_parameters.Clear(); \
		BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), PBR_X_PROXY_METHOD_PARAMS(method)) \
		BOOST_PP_IF(PBR_X_METHOD_HAS_RETURN(method), PBR_X_METHOD_CALL(method), PBR_X_METHOD_CALLWITHOUTRESULT(method)) \
	}

#define PBR_X_PROXY_METHOD_PARAMS(method) \
	BOOST_PP_SEQ_FOR_EACH_I(PBR_X_PROXY_METHOD_PARAM, ~, PBR_X_METHOD_PARAMS(method))

#define PBR_X_PROXY_METHOD_PARAM(r, data, i, param) \
	m_parameters.Add().PBR_X_PARAM_SET_FUNC(param) (BOOST_PP_CAT(p, i)); \

#define PBR_X_METHOD_CALL(method) \
	::ProtoBufRemote::PendingCall* call = m_client->Call(m_serviceName, BOOST_PP_STRINGIZE(PBR_X_METHOD_NAME(method)), \
		m_parameters); \
	call->Wait(); \
	BOOST_PP_IF(PBR_X_PARAM_IS_RESULT_PTR(PBR_X_METHOD_RETURN(method)), \
		call->GetResult()->PBR_X_PARAM_GET_FUNC(PBR_X_METHOD_RETURN(method))(resultPtr); \
		m_client->ReleaseCall(call); \
	, \
		PBR_X_PARAM_TYPE(PBR_X_METHOD_RETURN(method)) result \
			= call->GetResult()->PBR_X_PARAM_GET_FUNC(PBR_X_METHOD_RETURN(method))(); \
		m_client->ReleaseCall(call); \
		return result; \
	)

#define PBR_X_METHOD_CALLWITHOUTRESULT(method) \
	m_client->CallWithoutResult(m_serviceName, BOOST_PP_STRINGIZE(PBR_X_METHOD_NAME(method)), m_parameters);


//stub

#define PBR_X_STUB_METHOD(r, data, method) \
	virtual PBR_X_PARAM_RETURN_TYPE(PBR_X_METHOD_RETURN(method)) PBR_X_METHOD_NAME(method)( \
		PBR_X_DECLARE_METHOD_PARAMS(method)) = 0;

#define PBR_X_STUB_CALL_METHOD(r, data, method) \
	else if (strcmp(methodName, BOOST_PP_STRINGIZE(PBR_X_METHOD_NAME(method))) == 0) \
	{ \
		BOOST_PP_IF(PBR_X_METHOD_HAS_PARAMS(method), PBR_X_STUB_CHECK_PARAMS(method), \
			PBR_X_STUB_CHECK_NO_PARAMS(method)) \
	}

#define PBR_X_STUB_CHECK_PARAMS(method) \
	if (parameters.GetNumParameters() == BOOST_PP_SEQ_SIZE(PBR_X_METHOD_PARAMS(method))) \
	{ \
		if (BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_CHECK_PARAM, ~, PBR_X_METHOD_PARAMS(method))) \
		{ \
			PBR_X_STUB_MAKE_CALL(method) \
		} \
	}

#define PBR_X_STUB_CHECK_NO_PARAMS(method) \
	if (parameters.GetNumParameters() == 0) \
	{ \
		PBR_X_STUB_MAKE_CALL(method) \
	}

#define PBR_X_STUB_CHECK_PARAM(r, data, i, param) \
	BOOST_PP_EXPR_IF(i, &&) parameters.GetParameter(i).PBR_X_PARAM_CHECK_FUNC(param)()

#define PBR_X_STUB_MAKE_CALL(method) \
	BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_PREPARE_PARAM, ~, PBR_X_METHOD_PARAMS(method))) \
	BOOST_PP_IF(PBR_X_METHOD_HAS_RETURN(method), PBR_X_STUB_MAKE_CALL_WITH_RETURN(method), \
		PBR_X_STUB_MAKE_CALL_WITHOUT_RETURN(method))

#define PBR_X_STUB_PREPARE_PARAM(r, data, i, param) \
	BOOST_PP_IF(PBR_X_PARAM_CAN_GET_REF(param), \
		PBR_X_PARAM_PARAM_TYPE(param) BOOST_PP_CAT(p, i) = parameters.GetParameter(i).PBR_X_PARAM_GET_FUNC(param)(); \
	, \
		PBR_X_PARAM_TYPE(param) BOOST_PP_CAT(p, i); \
		parameters.GetParameter(i).PBR_X_PARAM_GET_FUNC(param)(&BOOST_PP_CAT(p, i)); \
	)

#define PBR_X_STUB_MAKE_CALL_WITH_RETURN(method) \
	BOOST_PP_IF(PBR_X_PARAM_IS_RESULT_PTR(PBR_X_METHOD_RETURN(method)), \
		PBR_X_STUB_MAKE_CALL_WITH_RETURN_PTR(method), \
		PBR_X_STUB_MAKE_CALL_WITH_RETURN_STD(method))

#define PBR_X_STUB_MAKE_CALL_WITH_RETURN_STD(method) \
	PBR_X_PARAM_TYPE(PBR_X_METHOD_RETURN(method)) resultValue = \
		PBR_X_METHOD_NAME(method)(BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_MAKE_CALL_PARAM, ~, PBR_X_METHOD_PARAMS(method)))); \
	result->PBR_X_PARAM_SET_FUNC(PBR_X_METHOD_RETURN(method))(resultValue); \
	return true;

#define PBR_X_STUB_MAKE_CALL_WITH_RETURN_PTR(method) \
	PBR_X_PARAM_TYPE(PBR_X_METHOD_RETURN(method)) resultValue; \
	PBR_X_METHOD_NAME(method)(BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_MAKE_CALL_PARAM, ~, PBR_X_METHOD_PARAMS(method))), &resultValue); \
	result->PBR_X_PARAM_SET_FUNC(PBR_X_METHOD_RETURN(method))(resultValue); \
	return true;

#define PBR_X_STUB_MAKE_CALL_WITHOUT_RETURN(method) \
	PBR_X_METHOD_NAME(method)(BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_MAKE_CALL_PARAM, ~, PBR_X_METHOD_PARAMS(method)))); \
	return true;

#define PBR_X_STUB_MAKE_CALL_PARAM(r, data, i, param) \
	BOOST_PP_COMMA_IF(i) BOOST_PP_CAT(p, i)

#endif

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
		name##Proxy(::ProtoBufRemote::RpcClient& client) : ::ProtoBufRemote::Proxy(client, #name) { } \
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

#define PBR_VOID ((void, 0, ~, ~, ~))
#define PBR_INT ((int, 1, SetInt, GetInt, IsInt))
#define PBR_BOOL ((bool, 1, SetBool, GetBool, IsBool))
#define PBR_STRING ((const std::string&, 1, SetString, GetString, IsString))


//=============================================================================================================
// internal macros
//=============================================================================================================

#define PBR_X_METHOD_NAME(method) BOOST_PP_TUPLE_ELEM(2, 0, method)
#define PBR_X_METHOD_SIG(method) BOOST_PP_TUPLE_ELEM(2, 1, method)
#define PBR_X_METHOD_RETURN(method) BOOST_PP_SEQ_ELEM(0, PBR_X_METHOD_SIG(method))
#define PBR_X_METHOD_PARAMS(method) BOOST_PP_SEQ_ELEM(1, PBR_X_METHOD_SIG(method))
#define PBR_X_METHOD_HAS_RETURN(method) BOOST_PP_TUPLE_ELEM(5, 1, PBR_X_METHOD_RETURN(method))
#define PBR_X_METHOD_HAS_PARAMS(method) BOOST_PP_TUPLE_ELEM(5, 1, BOOST_PP_SEQ_HEAD(PBR_X_METHOD_PARAMS(method)))

#define PBR_X_DECLARE_METHOD_PARAMS(method) \
	BOOST_PP_SEQ_FOR_EACH_I(PBR_X_DECLARE_METHOD_PARAM, ~, PBR_X_METHOD_PARAMS(method))

#define PBR_X_DECLARE_METHOD_PARAM(r, data, i, param) \
	BOOST_PP_COMMA_IF(i) BOOST_PP_TUPLE_ELEM(5, 0, param) BOOST_PP_CAT(p, i)

//proxy

#define PBR_X_PROXY_METHOD(r, data, method) \
	BOOST_PP_TUPLE_ELEM(5, 0, PBR_X_METHOD_RETURN(method)) PBR_X_METHOD_NAME(method)( \
		BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), PBR_X_DECLARE_METHOD_PARAMS(method))) \
	{ \
		m_parameters.Clear(); \
		BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), PBR_X_PROXY_METHOD_PARAMS(method)) \
		BOOST_PP_IF(PBR_X_METHOD_HAS_RETURN(method), PBR_X_METHOD_CALL(method), PBR_X_METHOD_CALLWITHOUTRESULT(method)) \
	}

#define PBR_X_PROXY_METHOD_PARAMS(method) \
	BOOST_PP_SEQ_FOR_EACH_I(PBR_X_PROXY_METHOD_PARAM, ~, PBR_X_METHOD_PARAMS(method))

#define PBR_X_PROXY_METHOD_PARAM(r, data, i, param) \
	m_parameters.Add(). BOOST_PP_TUPLE_ELEM(5, 2, param) (BOOST_PP_CAT(p, i)); \

#define PBR_X_METHOD_CALL(method) \
	::ProtoBufRemote::PendingCall* call = m_client.Call(m_serviceName, BOOST_PP_STRINGIZE(PBR_X_METHOD_NAME(method)), \
		m_parameters); \
	call->Wait(); \
	BOOST_PP_TUPLE_ELEM(5, 0, PBR_X_METHOD_RETURN(method)) result \
		= call->GetResult()->BOOST_PP_TUPLE_ELEM(5, 3, PBR_X_METHOD_RETURN(method))(); \
	m_client.ReleaseCall(call); \
	return result;

#define PBR_X_METHOD_CALLWITHOUTRESULT(method) \
	m_client.CallWithoutResult(m_serviceName, BOOST_PP_STRINGIZE(PBR_X_METHOD_NAME(method)), m_parameters);


//stub

#define PBR_X_STUB_METHOD(r, data, method) \
	virtual BOOST_PP_TUPLE_ELEM(5, 0, PBR_X_METHOD_RETURN(method)) PBR_X_METHOD_NAME(method)( \
		BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), PBR_X_DECLARE_METHOD_PARAMS(method))) = 0;

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
	BOOST_PP_EXPR_IF(i, &&) parameters.GetParameter(i).BOOST_PP_TUPLE_ELEM(5, 4, param)()

#define PBR_X_STUB_MAKE_CALL(method) \
	BOOST_PP_IF(PBR_X_METHOD_HAS_RETURN(method), PBR_X_STUB_MAKE_CALL_WITH_RETURN(method), \
		PBR_X_STUB_MAKE_CALL_WITHOUT_RETURN(method))

#define PBR_X_STUB_MAKE_CALL_WITH_RETURN(method) \
	BOOST_PP_TUPLE_ELEM(5, 0, PBR_X_METHOD_RETURN(method)) resultValue = \
		PBR_X_METHOD_NAME(method)(BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_MAKE_CALL_PARAM, ~, PBR_X_METHOD_PARAMS(method)))); \
	result->BOOST_PP_TUPLE_ELEM(5, 2, PBR_X_METHOD_RETURN(method))(resultValue); \
	return true;

#define PBR_X_STUB_MAKE_CALL_WITHOUT_RETURN(method) \
	PBR_X_METHOD_NAME(method)(BOOST_PP_EXPR_IF(PBR_X_METHOD_HAS_PARAMS(method), \
		BOOST_PP_SEQ_FOR_EACH_I(PBR_X_STUB_MAKE_CALL_PARAM, ~, PBR_X_METHOD_PARAMS(method)))); \
	return true;

#define PBR_X_STUB_MAKE_CALL_PARAM(r, data, i, param) \
	BOOST_PP_COMMA_IF(i) parameters.GetParameter(i).BOOST_PP_TUPLE_ELEM(5, 3, param)()

#endif

#ifndef PRIMESERVER_SAMPLESERVICEPROXY_H_
#define PRIMESERVER_SAMPLESERVICEPROXY_H_ 1

//
//Implement the proxy to call the prime testing ISampleService. Naming is chosen to match the C# sample.
//
//There are two alternative implementations for demonstration purposes, one using the Boost preprocessor library and
//one plain C++ implementation. Use the following define to switch between them.
//#define SAMPLE_WITH_BOOST 1



#if SAMPLE_WITH_BOOST

//
//Implementation of proxy using the Boost Preprocessor library
//

#include "ProtoBufRemote/Generators.h"

PBR_SERVICE(ISampleService,
    PBR_METHOD(TestPrime, PBR_BOOL(PBR_INT))
)




#else //SAMPLE_WITH_BOOST

//
//Implementation of proxy without using Boost
//

#include "ProtoBufRemote/PendingCall.h"
#include "ProtoBufRemote/Proxy.h"

class ISampleServiceProxy : public ProtoBufRemote::Proxy
{
public:
	ISampleServiceProxy(ProtoBufRemote::RpcClient* client)
        : ProtoBufRemote::Proxy(client, "ISampleService")
    {
    }

    bool TestPrime(int x)
    {
        m_parameters.Clear();
        m_parameters.Add().SetInt(x);
        ProtoBufRemote::PendingCall* call = m_client->Call(m_serviceName, "TestPrime", m_parameters);
        call->Wait();
        return call->GetResult()->GetBool();
    }
};


#endif //SAMPLE_WITH_BOOST


#endif

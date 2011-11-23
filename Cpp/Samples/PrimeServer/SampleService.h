#ifndef PRIMESERVER_SAMPLESERVICE_H_
#define PRIMESERVER_SAMPLESERVICE_H_ 1

//
//Implement the prime testing ISampleService. Naming is chosen to match the C# sample.
//
//There are two alternative implementations for demonstration purposes, one using the Boost preprocessor library and
//one plain C++ implementation. Use the following define to switch between them.
#define SAMPLE_WITH_BOOST 1



#if SAMPLE_WITH_BOOST

//
//Implementation of sample service using the Boost Preprocessor library
//

#include "ProtoBufRemote/Generators.h"

PBR_SERVICE(ISampleService,
    PBR_METHOD(TestPrime, PBR_BOOL(PBR_INT))
)

class SampleService : public ISampleServiceStub
{
public:
    virtual bool TestPrime(int x)
    {
        if (x == 1)
            return false;
        if (x == 2)
            return true;
        if ((x & 1) == 0)
            return false;
        for (int divisor=3; divisor<x; divisor+=2)
        {
            if ((x % divisor) == 0)
                return false;
        }
        return true;
    }
};





#else //SAMPLE_WITH_BOOST

//
//Implementation of sample service without using Boost
//

#include "ProtoBufRemote/MutableParameterList.h"
#include "ProtoBufRemote/RpcService.h"

class SampleService : public ProtoBufRemote::RpcService
{
public:
	SampleService()
        : ProtoBufRemote::RpcService("ISampleService")
    {
    }

    virtual bool Call(const char* methodName, const ProtoBufRemote::ParameterList& parameters,
                      ProtoBufRemote::MutableParameter* result)
    {
        if (_stricmp(methodName, "TestPrime") == 0)
        {
            if (parameters.GetNumParameters() == 1 && parameters.GetParameter(0).IsInt())
            {
                result->SetBool(TestPrime(parameters.GetParameter(0).GetInt()));
                return true;
            }
        }
        return false;
    }

    bool TestPrime(int x)
    {
        if (x == 1)
            return false;
        if (x == 2)
            return true;
        if ((x & 1) == 0)
            return false;
        for (int divisor=3; divisor<x; divisor+=2)
        {
            if ((x % divisor) == 0)
                return false;
        }
        return true;
    }
};


#endif //SAMPLE_WITH_BOOST


#endif

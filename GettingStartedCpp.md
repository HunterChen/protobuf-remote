# Creating the service definition #

The preprocessor can be used to generate a stub for the service and a proxy to make calls.

```
#include "ProtoBufRemote/Generators.h"

PBR_SERVICE(SampleService,
    PBR_METHOD(GetSquare, PBR_INT(PBR_INT))
    PBR_METHOD(GetDate, PBR_STRING(PBR_VOID))
    PBR_METHOD(SetName, PBR_VOID(PBR_STRING))
)
```

This defines two classes - SampleServiceStub, which can be used as the base class for the service implementation, and SampleServiceProxy, which can be used as is to make calls to the service.

If only a service stub is required, then the macro PBR\_SERVICE\_STUB can be used. Conversely if only a proxy is required, then the macro PBR\_PROXY can be used.

## Creating the service and proxy explicitly ##

Alternatively, instead of using the preprocessor macros, the service and proxy can be explicitly defined. See the [manual](ManualCpp.md) for more details.

# Creating a RPC server #

First the service should be implemented:
```
class SampleService : public SampleServiceStub
{
public:
    virtual int GetSquare(int x) { return x*x; }
    virtual std::string GetDate() { return "Today"; }
    virtual void SetName(const std::string& name) { m_name = name; }
};

```

Then the server can be created to handle requests to this service:
```
//create the server
m_rpcController = new RpcController;
m_rpcServer = new RpcServer(m_rpcController);
m_rpcChannel = new SocketRpcChannel(m_rpcController, socket);

//create and register the service with the rpc server
m_service = new SampleService;
m_rpcServer->RegisterService(m_service);

//now start the channel to begin receiving requests
m_rpcChannel->Start();
```

# Creating a RPC client #

First we create the client which will be used to send the requests:
```
//create the client
m_rpcController = new RpcController;
m_rpcClient = new RpcClient(m_rpcController);
m_rpcChannel = new SocketRpcChannel(m_rpcController, socket);
m_rpcChannel->Start();
```

Now a proxy can be created to make calls to a particular service:
```
SampleServiceProxy* proxy = new SampleServiceProxy(m_rpcClient);

//now calls can be made, they will block until a result is received
int x = proxy->GetSquare(10);
std::string date = proxy->GetDate();
proxy->SetName("John");
```
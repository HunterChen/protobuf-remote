# Creating the service definition #

An interface can optionally be used to define the service. It can then be used by both the server and the client.

```
public interface ISampleService
{
    int GetSquare(int x);
    string GetDate();
}
```


# Creating a RPC server #

First the service should be implemented:
```
public class SampleService : ISampleService
{
    public int GetSquare(int x)
    {
        return x*x;
    }

    public string GetDate()
    {
        return DateTime.Now.ToLongDateString();
    }
}
```

Then the server can be created to handle requests to this service:
```
//create the server
var controller = new RpcController();
server = new RpcServer(controller);

//register the service with the server. We must specify the interface explicitly since we did not use attributes
server.RegisterService<ISampleService>(new SampleService());

//create and start the channel which will receive requests
var channel = new NetworkStreamRpcChannel(controller, networkStream);
channel.Start();
```

# Creating a RPC client #

First we create the client which will be used to send the requests:
```
//create the client
var controller = new RpcController();
client = new RpcClient(controller);

//create and start the channel which will receive requests
var channel = new NetworkStreamRpcChannel(controller, networkStream);
channel.Start();
```

Now a proxy can be created to make calls to a particular service:
```
ISampleService service = client.GetProxy<ISampleService>();

//now calls can be made, they will block until a result is received
int x = service.GetSquare(10);
string date = service.GetDate();
```

#include <ws2tcpip.h>

#include "ProtoBufRemote/RpcClient.h"
#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/SocketRpcChannel.h"

#include "SampleServiceProxy.h"

using ProtoBufRemote::RpcClient;
using ProtoBufRemote::RpcController;
using ProtoBufRemote::SocketRpcChannel;

int main()
{
	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2,2), &wsaData);

	ADDRINFO hints;
	ADDRINFO* addrResult;
	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;
	result = getaddrinfo("127.0.0.1", "13000", &hints, &addrResult);

	SOCKET connectSocket = socket(addrResult->ai_family, addrResult->ai_socktype, addrResult->ai_protocol);
	result = connect(connectSocket, addrResult->ai_addr, (int)addrResult->ai_addrlen);

    RpcController* controller = new RpcController();
    RpcClient* client = new RpcClient(controller);

    SocketRpcChannel* channel = new SocketRpcChannel(controller, connectSocket);
    channel->Start();

    ISampleServiceProxy* proxy = new ISampleServiceProxy(client);

    int counter = 0;
    while (true)
    {
        printf("Enter number to test:\n");
        int x;
        while (scanf_s("%d", &x) != 1) { }

        printf(" Asking server if %d is prime...\n", x);
        bool isPrime = proxy->TestPrime(x);

        if (isPrime)
            printf(" Server says: Prime!\n\n");
        else
            printf(" Server says: Not prime!\n\n");
    }

    result = WSACleanup();

    return 0;
}

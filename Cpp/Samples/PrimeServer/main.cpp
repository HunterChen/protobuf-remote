
#include <ws2tcpip.h>

#include "ProtoBufRemote/RpcController.h"
#include "ProtoBufRemote/RpcServer.h"
#include "ProtoBufRemote/SocketRpcChannel.h"

#include "SampleService.h"


using ProtoBufRemote::RpcController;
using ProtoBufRemote::RpcServer;
using ProtoBufRemote::SocketRpcChannel;


int main()
{
   	WSADATA wsaData;
	int result = WSAStartup(MAKEWORD(2,2), &wsaData);

    ADDRINFO hints;
    ADDRINFO* addrResult;
    ZeroMemory(&hints, sizeof(hints));
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    hints.ai_protocol = IPPROTO_TCP;
    hints.ai_flags = AI_PASSIVE;
    result = getaddrinfo(NULL, "13000", &hints, &addrResult);

    SOCKET listenSocket = socket(addrResult->ai_family, addrResult->ai_socktype, addrResult->ai_protocol);

    result = bind(listenSocket, addrResult->ai_addr, (int)addrResult->ai_addrlen);

    result = listen(listenSocket, SOMAXCONN);

    while (true)
    {
        printf("Waiting for a connection...\n");

        SOCKET clientSocket = accept(listenSocket, NULL, NULL);

        printf("Connected!\n");

        RpcController* controller = new RpcController();
        RpcServer* server = new RpcServer(controller);

        SampleService* service = new SampleService();
        server->RegisterService(service);

        SocketRpcChannel* channel = new SocketRpcChannel(controller, clientSocket);
        channel->Start();

        while (!channel->IsClosed())
            Sleep(1000);

        channel->CloseAndJoin();

        delete channel;
        delete server;
        delete service;
        delete controller;

        printf("Connection closed.\n\n");
    }

    freeaddrinfo(addrResult);
	closesocket(listenSocket);
	
	result = WSACleanup();

    return 0;
}

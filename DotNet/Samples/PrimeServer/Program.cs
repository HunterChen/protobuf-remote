using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ProtoBufRemote;

namespace PrimeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            TcpListener tcpListener = new TcpListener(localAddr, port);
            tcpListener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for a connection...");

                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                Console.WriteLine("Connected!");

                var controller = new RpcController();
                var server = new RpcServer(controller);

                server.RegisterService<ISampleService>(new SampleService());

                var channel = new NetworkStreamRpcChannel(controller, tcpClient.GetStream());
                channel.Start();

                while (tcpClient.Connected)
                    System.Threading.Thread.Sleep(1000);

                channel.CloseAndJoin();

                Console.WriteLine("Connection closed.\n");
            }
        }
    }
}

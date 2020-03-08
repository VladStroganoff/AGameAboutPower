using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    static class ServerTCP
    {
        static TcpListener serverSocket = new TcpListener(IPAddress.Any, 5587);

        public static void InitializeNetwork()
        {
            Console.WriteLine("Initializing Packets....");
            ServerHandleData.InitializePackets();
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);



            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("ip: " + ip.ToString()  + " socket: 55087" ) ;
                }
            }

        }

        private static void OnClientConnect(IAsyncResult result)
        {
            TcpClient client = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
            ClientManager.CreateNewConnection(client);
        }

    }
}

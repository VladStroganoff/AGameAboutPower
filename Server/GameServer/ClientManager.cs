using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace GameServer
{
    static class ClientManager
    {
        public static Dictionary<int, Client> Client = new Dictionary<int, Client>();

        public static void CreateNewConnection(TcpClient tempClient)
        {
            Client newClient = new Client();
            newClient.Socket = tempClient;
            newClient.ConnectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port;
            newClient.Start();
            Client.Add(newClient.ConnectionID, newClient);

            DataSender.SendWelcomeMessage(newClient.ConnectionID);
            InstasiatePlayer(newClient.ConnectionID);
        }


        public static void InstasiatePlayer(int connectionID)
        {
            foreach(var item in Client)
            {
                if(item.Key != connectionID)
                {
                    DataSender.SendInstansiatePlayer(item.Key, connectionID);
                }
            }

            foreach(var item in Client)
            {
                DataSender.SendInstansiatePlayer(connectionID, item.Key);
            }

        }

        public static void SendDataTo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
            buffer.WriteBytes(data);
            Client[connectionID].Stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
            buffer.Dispose();
        }
    }
}

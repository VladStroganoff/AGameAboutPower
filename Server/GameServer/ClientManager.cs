using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using GameServer.World;
using GameServer.Player;

namespace GameServer
{
    static class ClientManager
    {
        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();


        public static void CreateNewConnection(TcpClient tempClient)
        {
            Client newClient = new Client();
            newClient.Socket = tempClient;
            newClient.ConnectionID = ((IPEndPoint)tempClient.Client.RemoteEndPoint).Port;
            newClient.Start();
            Clients.Add(newClient.ConnectionID, newClient);

            DataSender.SendWelcomeMessage(newClient.ConnectionID);

            PlayerData newPlayer = new PlayerData();
            newPlayer.ConnectionID = newClient.ConnectionID;
            WorldController.instance.AddPlayerToWorld(newPlayer);
        }


        public static void NewPlayer(PlayerData player)
        {

            // send everyone to new player except himself

            foreach(KeyValuePair<int, Client> item in Clients)
            {
                if(item.Key != player.ConnectionID)
                {
                    DataSender.SendPlayer(player, item.Key);
                }
            }

            // send new player to everyone including himself

            foreach(KeyValuePair<int, Client> item in Clients)
            {
                DataSender.SendPlayer(WorldController.instance.Model.Players[item.Key], player.ConnectionID);
            }

        }

        public static void UpdatePlayer(PlayerData player)
        {
            foreach (KeyValuePair<int, Client> item in Clients)
            {
                if (item.Key != player.ConnectionID)
                {
                    DataSender.SendPlayer(player, item.Key);
                }
            }
        }

        public static void SendDataTo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(data.GetUpperBound(0) - data.GetLowerBound(0) + 1);
            buffer.WriteBytes(data);

            Clients[connectionID].Stream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);

            buffer.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GameServer.Player;
using GameServer.World;

namespace GameServer
{
    public enum ClientPackets
    {
        StringMessage = 1,
    }
    static class DataRecevier
    {
        public static void HandleStringMessage(int connectonID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInt();
            string message = buffer.ReadString();


            PlayerData player = new PlayerData();

            if (message != "Hello Server")
            {
                player = JsonConvert.DeserializeObject<PlayerData>(message);
                WorldController.instance.UpdatePlayerInWorld(player);
            }
            else
            {
                Console.Write(message);
            }
        }
    }
}

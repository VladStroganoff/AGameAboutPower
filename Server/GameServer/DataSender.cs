using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Player;


namespace GameServer
{


    public enum ServerPackets
    { 
        SWelcomeMessage = 1,
        SPlayerData =2,
    }


    static class DataSender
    {
        public static void SendWelcomeMessage(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ServerPackets.SWelcomeMessage);
            buffer.WriteString(connectionID.ToString());
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendPlayer (PlayerData player, int connectionID)
        {
            string json = JsonConvert.SerializeObject(player);
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ServerPackets.SPlayerData);
            buffer.WriteString(json);


            ClientManager.SendDataTo(connectionID, buffer.ToArray());

            buffer.Dispose();
        }


    }
}

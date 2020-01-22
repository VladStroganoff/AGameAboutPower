using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            buffer.WriteString("Hello, Welcome to the server!");
            ClientManager.SendDataTo(connectionID, buffer.ToArray());

            Console.WriteLine("message should be: " + buffer.ToArray().Length.ToString());

            buffer.Dispose();
        }

        public static void SendInstansiatePlayer (int index, int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ServerPackets.SPlayerData);
            buffer.WriteInt(index);
            ClientManager.SendDataTo(connectionID, buffer.ToArray());
            buffer.Dispose();
        }

    }
}

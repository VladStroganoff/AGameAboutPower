using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{

    public enum ClientPackets
    {
        CHelloServer = 1,
        PlayerUpdate = 2,
    }


    static class DataSender
    {
        public static void SendHelloServer()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ClientPackets.CHelloServer);
            buffer.WriteString("Thanks, Im now connected to you.");
            ClientTCP.SendingData(buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendPlayerUpdate(PlayerData playerData)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ClientPackets.PlayerUpdate);

            string json = UnityEngine.JsonUtility.ToJson(playerData);

            buffer.WriteString(json);
            ClientTCP.SendingData(buffer.ToArray());
            buffer.Dispose();
        }
    }
}

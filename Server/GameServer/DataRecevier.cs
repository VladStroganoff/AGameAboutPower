using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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



            if (message != "Hello Server")
            {
                NetEntity entity = new NetEntity();

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                entity = JsonConvert.DeserializeObject<NetEntity>(message, settings);

                WorldController.instance.UpdatePlayerInWorld(entity);
            }
            else
            {
                Console.Write(message);
            }
        }
    }
}

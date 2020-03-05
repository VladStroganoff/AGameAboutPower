using Newtonsoft.Json;

namespace Assets.Scripts
{

    public enum ClientPackets
    {
        CHelloServer = 1,
    }



    static class DataSender
    {
        public static void SendServerMessage(NetEntity myEntity)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string json = JsonConvert.SerializeObject(myEntity, settings);

            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ClientPackets.CHelloServer);
            buffer.WriteString(json);
            ClientTCP.SendingData(buffer.ToArray());
            buffer.Dispose();
        }

        public static void SendServerMessage(string message)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt((int)ClientPackets.CHelloServer);
            buffer.WriteString(message);
            ClientTCP.SendingData(buffer.ToArray());
            buffer.Dispose();
        }

    }
}

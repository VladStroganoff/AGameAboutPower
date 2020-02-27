using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Assets.Scripts
{
    static class DataReceiver
    {
        public enum ServerPackets
        {
            SWelcomeMessage = 1,
            SInstantiatePlayer =2,
        }

        public static void HandleWlcomeMessage(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInt();
            string message = buffer.ReadString();
            int id = 0;
            int.TryParse(message, out id);
            NetworkManager.instance.LocalPlayerConnectionID(id);
            FDebug.Log.Message("My connection is: " + id);
            FDebug.Log.Message(message);
            buffer.Dispose();

            DataSender.SendServerMessage("Hello Server");
        }

        public static void HandleInstansiateEntity(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInt();
            string message = buffer.ReadString();
            
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            NetEntity entity = JsonConvert.DeserializeObject<NetEntity>(message, settings);

            buffer.Dispose();

            FDebug.Log.Message("Instansiating player: " + entity.ConnectionID);
            NetworkManager.instance.HandleEntity(entity);
        }


    }
}

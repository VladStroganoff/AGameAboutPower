using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            Debug.Log("My connection is: " + id);
            Debug.Log(message);
            buffer.Dispose();

            DataSender.SendServerMessage("Hello Server");
        }

        public static void HandleInstansiateEntity(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInt();
            string message = buffer.ReadString();
            NetworkedEntity entity = JsonUtility.FromJson<NetworkedEntity>(message);
            buffer.Dispose();

            Debug.Log("Instansiating player: " + entity.ConnectionID);
            NetworkManager.instance.HandleEntity(entity);
        }


    }
}

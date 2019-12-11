using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    static class ServerHandleData
    {

        public delegate void Packet(int connectionID, byte[] data);
        public static Dictionary<int, Packet> Packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            Packets.Add((int)ClientPackets.CHelloServer, DataRecevier.HandleHelloServer);
        }

        public static void HandleData(int connectionID, byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int packetLength = 0;

            if (ClientManager.Client[connectionID].Buffer == null)
                ClientManager.Client[connectionID].Buffer = new ByteBuffer();

            ClientManager.Client[connectionID].Buffer.WriteBytes(buffer);

            if(ClientManager.Client[connectionID].Buffer.Count() == 0)
            {
                ClientManager.Client[connectionID].Buffer.Clear();
                return;
            }

            if(ClientManager.Client[connectionID].Buffer.Length() >= 4)
            {
                packetLength = ClientManager.Client[connectionID].Buffer.ReadInt(false);
                if(packetLength <= 0)
                {
                    ClientManager.Client[connectionID].Buffer.Clear();
                    return;
                }
            }

            while(packetLength > 0 && packetLength <= ClientManager.Client[connectionID].Buffer.Length() -4)
            {
                if(packetLength <= ClientManager.Client[connectionID].Buffer.Length() - 4)
                {
                    ClientManager.Client[connectionID].Buffer.ReadInt();
                    data = ClientManager.Client[connectionID].Buffer.ReadByteArray(packetLength);
                    HandleDataPacket(connectionID, data);
                }

                packetLength = 0;

                if(ClientManager.Client[connectionID].Buffer.Length() >=4)
                {
                    packetLength = ClientManager.Client[connectionID].Buffer.ReadInt(false);
                    if(packetLength <= 0)
                    {
                        ClientManager.Client[connectionID].Buffer.Clear();
                        return;
                    }
                }

            }

            if(packetLength <= 1)
            {
                ClientManager.Client[connectionID].Buffer.Clear();
            }
        }

        private static void HandleDataPacket(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInt();
            buffer.Dispose();
            if(Packets.TryGetValue(packetID, out Packet packet))
            {
                packet.Invoke(connectionID, data);
            }
        }

    }
}

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
        static int i = 0;


        public static void InitializePackets()
        {
            Packets.Add((int)ClientPackets.CHelloServer, DataRecevier.HandleHelloServer);
            Packets.Add((int)ClientPackets.PlayerUpdate, DataRecevier.HandleHelloServer);
        }

        public static void HandleData(int connectionID, byte[] data)
        {
            i++;

            byte[] buffer = (byte[])data.Clone();
            int packetLength = 0;

            if (ClientManager.Clients[connectionID].Buffer == null)
                ClientManager.Clients[connectionID].Buffer = new ByteBuffer();

            ClientManager.Clients[connectionID].Buffer.WriteBytes(buffer);

            if(ClientManager.Clients[connectionID].Buffer.Count() == 0)
            {
                ClientManager.Clients[connectionID].Buffer.Clear();
                return;
            }

            if(ClientManager.Clients[connectionID].Buffer.Length() >= 4)
            {
                packetLength = ClientManager.Clients[connectionID].Buffer.ReadInt(false);
                if(packetLength <= 0)
                {
                    ClientManager.Clients[connectionID].Buffer.Clear();
                    return;
                }
            }

            while(packetLength > 0 & packetLength <= ClientManager.Clients[connectionID].Buffer.Length() -4)
            {
                if(packetLength <= ClientManager.Clients[connectionID].Buffer.Length() - 4)
                {
                    ClientManager.Clients[connectionID].Buffer.ReadInt();
                    data = ClientManager.Clients[connectionID].Buffer.ReadByteArray(packetLength);
                    HandleDataPacket(connectionID, data);
                    ClientManager.Clients[connectionID].Buffer.Clear();
                }

                packetLength = 0;

                if(ClientManager.Clients[connectionID].Buffer.Length() >=4)
                {
                    packetLength = ClientManager.Clients[connectionID].Buffer.ReadInt(false);
                    if(packetLength <= 0)
                    {
                        ClientManager.Clients[connectionID].Buffer.Clear();
                        return;
                    }
                }

            }

            if(packetLength <= 1)
            {
                ClientManager.Clients[connectionID].Buffer.Clear();
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

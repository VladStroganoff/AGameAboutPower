using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.DataReceiver;

namespace Assets.Scripts
{
    static class ClientHandleData
    {
        private static ByteBuffer playerBuffer;
        public delegate void Packet(byte[] data);
        public static Dictionary<int, Packet> Packets = new Dictionary<int, Packet>();

        public static void InitializePackets()
        {
            Packets.Add((int)ServerPackets.SInstantiatePlayer, DataReceiver.HandleInstansiatePlayer);
            Packets.Add((int)ServerPackets.SWelcomeMessage, DataReceiver.HandleWlcomeMessage);
        }

        public static void HandleData(byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int packetLength = 0;

            if (playerBuffer == null)
                playerBuffer = new ByteBuffer();

            playerBuffer.WriteBytes(buffer);

            if (playerBuffer.Count() == 0)
            {
                playerBuffer.Clear();
                return;
            }

            if (playerBuffer.Length() >= 4)
            {
                packetLength = playerBuffer.ReadInt(false);
                if (packetLength <= 0)
                {
                    playerBuffer.Clear();
                    return;
                }
            }

            while (packetLength > 0 && packetLength <= playerBuffer.Length() - 1)
            {
                if (packetLength <= playerBuffer.Length() - 4)
                {
                    playerBuffer.ReadInt();
                    data = playerBuffer.ReadByteArray(packetLength);
                    HandleDataPacket(data);
                }

                packetLength = 0;

                if (playerBuffer.Length() >= 4)
                {
                    packetLength = playerBuffer.ReadInt(false);
                    if (packetLength <= 0)
                    {
                        playerBuffer.Clear();
                        return;
                    }
                }

            }

            if (packetLength <= 1)
            {
                playerBuffer.Clear();
            }
        }

        private static void HandleDataPacket(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            int packetID = buffer.ReadInt();
            buffer.Dispose();
            if (Packets.TryGetValue(packetID, out Packet packet))
            {
                packet.Invoke(data);
            }
        }

    }
}

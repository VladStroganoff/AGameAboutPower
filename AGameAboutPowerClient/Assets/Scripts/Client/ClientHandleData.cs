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

        private static int refLengt = 0;

        public static void InitializePackets()
        {
            Packets.Add((int)ServerPackets.SInstantiatePlayer, DataReceiver.HandleInstansiatePlayer);
            Packets.Add((int)ServerPackets.SWelcomeMessage, DataReceiver.HandleWlcomeMessage);
        }

        public static void HandleData(byte[] data)
        {
            byte[] buffer = (byte[])data.Clone();
            int packetLength = 0;

            NetworkManager.instance.ErrorMessage(buffer.Length.ToString());

            playerBuffer = new ByteBuffer();
            refLengt = 0;

            playerBuffer.WriteBytes(buffer);

            if (playerBuffer.Count() == 0) //is buffer empty?
            {
                playerBuffer.Clear();
                return;
            }

            if (playerBuffer.Length() >= 4) // is buffer marked with length 0?
            {
                packetLength = playerBuffer.ReadInt(false);
                refLengt = packetLength;
                if (packetLength <= 0)
                {
                    playerBuffer.Clear();
                    return;
                }
            }

            while (packetLength > 0 && packetLength <= playerBuffer.Length() - 1)  // this needs to execute before the above HandleData
            {
                if (packetLength <= playerBuffer.Length() - 4)
                {
                    playerBuffer.ReadInt();
                    data = playerBuffer.ReadByteArray(packetLength);
                    HandleDataPacket(data);
                    playerBuffer.Clear();
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

            if (refLengt < data.Length)
            {
                byte[] otherHalf = new byte[data.Length - refLengt];


                int j = 0;
                for(int i = refLengt; i < data.Length; i++)
                {
                    otherHalf[j] = data[i - 4];
                    j++;
                }

                HandleData(otherHalf);
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

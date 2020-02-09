using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



            Console.WriteLine(message);
        }
    }
}

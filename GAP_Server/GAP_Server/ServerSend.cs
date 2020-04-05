using System;
using System.Collections.Generic;
using System.Text;

namespace GAP_Server
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();

            for(int i = 0; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }

        }


        private static void SendTCPDataToAll(int _exception,Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                if(i != _exception)
                    Server.clients[i].tcp.SendData(_packet);
            }

        }


        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

    }
}

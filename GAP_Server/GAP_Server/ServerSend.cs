using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GAP_Server
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendUDPData(int toClient, Packet packet)
        {
            packet.WriteLength();
            Server.clients[toClient].udp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();

            for(int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }

        }


        private static void SendTCPDataToAll(int _exception,Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if(i != _exception)
                    Server.clients[i].tcp.SendData(_packet);
            }

        }

        private static void SendUDPDataToAll(Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(_packet);
            }

        }


        private static void SendUDPDataToAll(int _exception, Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i != _exception)
                    Server.clients[i].udp.SendData(_packet);
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

        public static void SpawnPlayer(int toClient, Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnplayer))
            {
                packet.Write(player.ID);
                packet.Write(player.name);
                packet.Write(player.position);
                packet.Write(player.rotation);

                SendTCPData(toClient, packet);
            }
        }

        public static void PlayerPosition(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerPosition))
            {
                packet.Write(player.ID);
                packet.Write(player.position);
                SendUDPDataToAll(packet);
            }
        }

        public static void PlayerRotation(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerRotation))
            {
                packet.Write(player.ID);
                packet.Write(player.rotation);
                SendUDPDataToAll(player.ID, packet);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using Newtonsoft.Json;

public class ServerSend
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

        for (int i = 1; i < Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }

    }


    private static void SendTCPDataToAll(int _exception, Packet _packet)
    {
        _packet.WriteLength();

        for (int i = 1; i < Server.MaxPlayers; i++)
        {
            if (i != _exception)
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
            packet.Write(player.transform.position);
            packet.Write(player.transform.rotation);
            SendTCPData(toClient, packet);
        }
    }

    public static void PlayerPosition(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerPosition))
        {
            packet.Write(player.ID);
            packet.Write(player.transform.position);
            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerRotation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRotation))
        {
            Debug.Log("Rotation: " + player.transform.rotation);
            packet.Write(player.ID);
            packet.Write(player.transform.rotation);
            SendUDPDataToAll(packet);
        }
    }

    public static void PlayerHealth(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerHEalth))
        {
            packet.Write(player.ID);
            packet.Write(player.health);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerAnimation(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.jsonObject))
        {
            packet.Write(player.ID);

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string json = JsonConvert.SerializeObject(player.animator, settings);
            packet.Write(json);

            SendTCPDataToAll(packet);
        }
    }

    public static void BuildStructure( int player, BuildingData building)
    {
        using (Packet packet = new Packet((int)ServerPackets.jsonObject))
        {
            packet.Write(player);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            string json = JsonConvert.SerializeObject(building, settings);
            packet.Write(json);
            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerRespawn(Player player)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerRespawn))
        {
            packet.Write(player.ID);

            SendTCPDataToAll(packet);
        }
    }

    public static void PlayerDisconnected(int playerId)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDisconnect))
        {
            packet.Write(playerId);
            SendTCPDataToAll(packet);
        }
    }

}

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerHandle
{
    public static void WelcomeRecieved(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string playerData = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{playerData}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(playerData);

    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }

        Quaternion rotation = packet.ReadQuaternion();

        Server.clients[fromClient].player.SetInput(inputs, rotation);
    }

    public static void PlayerShoot(int fromPlayer, Packet packet)
    {
        Vector3 shootDirection = packet.ReadVector3();
        Server.clients[fromPlayer].player.Shoot(shootDirection);
    }

    public static void JsonPackate(int fromPlayer, Packet packet)
    {
        int id = packet.ReadInt();
        string json = packet.ReadString();
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        NetEntity netPackage = JsonConvert.DeserializeObject<NetEntity>(json, settings);
        
        if(netPackage is BuildingData)
        {
            NetworkManager.instance.ConstructionControl.BuildBuilding(id, (BuildingData)netPackage);
        }
        if (netPackage is NetItem)
        {
            NetItem netItem = netPackage as NetItem;

            if(netItem is Netwearable)
            {
                Netwearable netWear = netItem as Netwearable;
                Debug.Log($"got wearable: {netWear.Name}");
                NetworkManager.instance.InventoryControl.ChangeWear(netWear, id);
            }
            if(netItem is NetHoldable)
            {
                NetHoldable netHold = netItem as NetHoldable;
                Debug.Log($"got holdable: {netHold.Name}");
            }
        }
        if (netPackage is NetLoot)
        {
            NetLoot netLoot = netPackage as NetLoot;

            if (netLoot.ownerID == -1)
                NetworkManager.instance.LootControl.UpdateLoot(netLoot);
            else
                NetworkManager.instance.InventoryControl.UpdatePlayerInventory(netLoot);
        }


    }

}

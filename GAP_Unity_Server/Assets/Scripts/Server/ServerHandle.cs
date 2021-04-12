using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerHandle
{
    public static void WelcomeRecieved(int fromClient, Packet packet)
    {
        int clientIDCheck = packet.ReadInt();
        string welcomeMessage = packet.ReadString();

        Debug.Log("PLayer: " + welcomeMessage + " joined the server.");

        if (fromClient != clientIDCheck)
        {
            Debug.Log("there's a mole... at the highest level of the Circus...");
        }

        Server.clients[fromClient].SendIntoGame(welcomeMessage);

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
            NetworkManager.instance.ConstructionControl.BuildBuilding(id, (BuildingData)netPackage);
        if (netPackage is NetAnimator)
            Debug.Log("was animator");
    }

}

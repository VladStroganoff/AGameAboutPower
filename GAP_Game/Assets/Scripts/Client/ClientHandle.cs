using UnityEngine;
using System.Net;
using Newtonsoft.Json;



public class ClientHandle : MonoBehaviour
{

    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();
        Debug.Log(_msg + "from port: " + ((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotaton = packet.ReadQuaternion();
        GameManager.instance.SpawnPlayer(id, username, position, rotaton);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        //Debug.Log($"Got Player Movement{position}");
        GameManager.players[id].transform.position = position;
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();
        GameManager.players[id].transform.rotation = rotation;
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        float health = packet.ReadFloat();
        GameManager.players[id].SetHealth(health);
    }

    public static void PlayerRespawned(Packet packet)
    {
        int id = packet.ReadInt();
        GameManager.players[id].Respawn();
    }

    public static void JsonObject(Packet packet)
    {
        int id = packet.ReadInt();

        if (!GameManager.players.ContainsKey(id))
            return;

        string json = packet.ReadString();
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        NetEntity jsonObj = JsonConvert.DeserializeObject<NetEntity>(json, settings);

        if(jsonObj is NetAnimator)
            GameManager.players[id].Animator.Set(jsonObj as NetAnimator);
        if (jsonObj is BuildingData)
            GameManager.instance.SpawnStructure(jsonObj as BuildingData);
    }
}

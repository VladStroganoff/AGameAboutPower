using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

public class ClientHandle : MonoBehaviour
{
  public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myID = _packet.ReadInt();


        Debug.Log("Da message was: " + _msg);
        Client.instance.myId = _myID;

        ClientSend.WelcomeReceived();

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

}

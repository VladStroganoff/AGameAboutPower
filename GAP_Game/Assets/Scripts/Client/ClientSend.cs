﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);

    }


    public static void SendUDP(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(packet);
        }
    }

    public static void SendPlayerMovement(bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(inputs.Length);
            foreach (bool input in inputs)
            {
                packet.Write(input);
            }

            Vector3 rotation = GameManager.players[Client.instance.myId].Camera.transform.rotation.eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            Quaternion actualRot = Quaternion.Euler(GameManager.players[Client.instance.myId].Camera.transform.rotation.eulerAngles);

            packet.Write(actualRot);
            //Debug.Log(inputs[0] + inputs[1].ToString() + inputs[2] + inputs[3]);
            SendUDP(packet);
        }
    }

    public static void PlayerShoot(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }

}

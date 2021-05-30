using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        GameClient.instance.tcp.SendData(packet);
    }

    private static void SendUDP(Packet packet)
    {
        packet.WriteLength();
        GameClient.instance.udp.SendData(packet);
    }

    public static void WelcomeReceived()
    {

        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(GameClient.instance.myId);

            if (UIManager.instance != null)
                packet.Write(LoadController.instance.GetInventoryJson());
            else
                packet.Write("Test Join");

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

            Quaternion camRot = Quaternion.Euler(GameManager.players[GameClient.instance.myId].Camera.transform.rotation.eulerAngles);

            packet.Write(camRot);
            SendUDP(packet);
        }
    }

    public static void SendJsonPackage(string json)
    {
        using (Packet packet = new Packet((int)ClientPackets.jsonObject))
        {
            packet.Write(GameClient.instance.myId);
            packet.Write(json);
            SendTCPData(packet);
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

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public PlayerData localPlayer;

    public Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();
    public bool updatePlayer { get; set; }

    public PlayerController PlayerControl;
    public InputField IpField;
    public InputField PortField;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();


        IpField.text = "10.0.0.4";
        PortField.text = "5587";
    }

    public void TryInitializeConnection()
    {

        ClientHandleData.InitializePackets();
        ClientTCP.InitializeNetworking(IpField.text, int.Parse(PortField.text));
    }


    private void OnApplicationQuit()
    {
        ClientTCP.Disconnect();
    }

    public void UpdatePlayerPosition(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        DataSender.SendServerMessage(json);
    }

    public void HandlePlayer(PlayerData player)
    {
        if(PlayerList.Count > 1 || PlayerList.ContainsKey(player.ConnectionID))
        {
            PlayerControl.UpdatePlayer(player);
        }
        else
        {
            PlayerControl.InstansiateNewPlayer(player);
        }
    }

    public void LocalPlayerConnectionID(int id)
    {
        PlayerData newPlayer = new PlayerData();
        newPlayer.ConnectionID = id;
        localPlayer = newPlayer;
    }

    public void ErrorMessage(string msg)
    {
        Debug.Log(msg);
    }


   

    IEnumerator SendUpdate()
    {
        while(updatePlayer)
        {
            localPlayer = Make.PlayerUpdate(localPlayer, PlayerList[localPlayer.ConnectionID].transform.GetChild(0).transform);

            string json = JsonUtility.ToJson(localPlayer);

            DataSender.SendServerMessage(json);

            yield return new WaitForSeconds(3);
        }
    }
}
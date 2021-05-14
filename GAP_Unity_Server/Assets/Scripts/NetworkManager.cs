﻿using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager instance;
    public GameObject PlayerPrefab;
    public Transform PlayerSpawnPoint;
    public ConstructionController ConstructionControl;
    public InventoryController InventoryControl;
    public int Port;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, Port);

    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }


    public Player InstantiatePlayer(string playerData)
    {

        GameObject playerPrefab = Instantiate(PlayerPrefab, PlayerSpawnPoint.position, Quaternion.identity);
        Player player = playerPrefab.GetComponent<Player>();
        player.GetComponent<InventoryModel>().InitializeInventory(playerData, player);

        return player;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;


    public GameObject PlayerPrefab;

    public Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);
        UnityThread.initUnityThread();
        ClientHandleData.InitializePackets();
        ClientTCP.InitializeNetworking();

    }


    private void OnApplicationQuit()
    {
        ClientTCP.Disconnect();
    }


    public void InstantiatePlayer(int index)
    {
        GameObject pureyeruh = Instantiate(PlayerPrefab);
        pureyeruh.name = "Player: " + index;
        PlayerList.Add(index, pureyeruh);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject PlayerPrefab;
    public int localPlayerID;

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

    public void UpdatePlayerPosition(PlayerData data)
    {
        DataSender.SendPlayerUpdate(data);
    }

    public void InstantiatePlayer(int index)
    {
        GameObject player = Instantiate(PlayerPrefab);
        player.name = "Player: " + index;
        PlayerList.Add(index, player);
        player.GetComponent<PlayerNetworkController>().Inject(localPlayerID, index);
        player.GetComponent<PlayerNameSignView>().Inject(index);

    }

    public void LocalPlayerConnectionID(int id)
    {
        localPlayerID = id;
    }

    public void ErrorMessage(string msg)
    {
        Debug.Log(msg);
    }
}

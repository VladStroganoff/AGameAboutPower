using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject PlayerPrefab;
    public int localPlayerID;
    public GameObject player;

    public Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();
    bool updatePlayer;

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
        string json = JsonUtility.ToJson(data);
        DataSender.SendServerMessage(json);
    }

    public void InstantiatePlayer(int index)
    {
        player = Instantiate(PlayerPrefab);


        if (localPlayerID != index)
        {
            Destroy(player.GetComponent<PlayerNetworkController>().Camera.gameObject);
            player.GetComponent<PlayerNetworkController>().Inject(false);
        }
        else
        {
            player.GetComponent<PlayerNetworkController>().Inject(true);
        }


        player.name = "Player: " + index;
        PlayerList.Add(index, player);
        player.GetComponent<PlayerNameSignView>().Inject(index);


        updatePlayer = true;
        StartCoroutine("SendUpdate");

    }

    public void LocalPlayerConnectionID(int id)
    {
        localPlayerID = id;
    }

    public void ErrorMessage(string msg)
    {
        Debug.Log(msg);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    IEnumerator SendUpdate()
    {
        while(updatePlayer)
        {
            PlayerData player = Make.NewPlyer(PlayerList[localPlayerID].transform.GetChild(0).transform, localPlayerID, "Player: " + localPlayerID.ToString()); // I want to keep construction out of the player class for it to be used both on client and server. 

            string json = JsonUtility.ToJson(player);

            DataSender.SendServerMessage(json);

            yield return new WaitForSeconds(3);
        }
    }
}

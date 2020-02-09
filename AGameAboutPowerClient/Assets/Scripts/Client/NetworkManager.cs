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
        Quaternion q = new Quaternion();
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
            PlayerData data = new PlayerData(); // just testing player update 
            data.ConnectionID = localPlayerID;
            data.Name = player.gameObject.name;
            data.Xpos = PlayerList[localPlayerID].transform.GetChild(0).position.x;
            data.Ypos = PlayerList[localPlayerID].transform.GetChild(0).position.y;
            data.Zpos = PlayerList[localPlayerID].transform.GetChild(0).position.y;

            string json = JsonUtility.ToJson(data);

            DataSender.SendServerMessage(json);

            yield return new WaitForSeconds(1);
        }
    }
}

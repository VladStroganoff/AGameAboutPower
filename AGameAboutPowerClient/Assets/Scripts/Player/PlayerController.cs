using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class PlayerController : MonoBehaviour
{
    public int MyConnectionID;
    public NetEntity localPlayer;
    public bool updatePlayer { get; set; }


    public GameObject PlayerPrefab;
    public GameObject Camera;
    public GameObject Player;

    public Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();


    public void HandlePlayer(NetEntity player)
    {
        if (PlayerList.Count > 1 || PlayerList.ContainsKey(NetworkManager.instance.ConnectionID))
        {
            UpdatePlayer(player);
        }
        else
        {
            InstansiateNewPlayer(player);
        }
    }


    public void InstansiateNewPlayer(NetEntity player)
    {
        GameObject playerGO = Instantiate(PlayerPrefab);

        if (MyConnectionID != NetworkManager.instance.ConnectionID)
        {
            playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().isLocalPlayer = false;
        }
        else
        {
            playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().isLocalPlayer = true;
            Player = playerGO;
            SetupCamera(playerGO);
        }


        playerGO.name = "Player: " + NetworkManager.instance.ConnectionID;
        playerGO.GetComponent<PlayerNameSignView>().Inject(NetworkManager.instance.ConnectionID);


        PlayerList.Add(NetworkManager.instance.ConnectionID, playerGO);
        localPlayer = player;


        updatePlayer = true;
        StartCoroutine("SendUpdate");
    }

    void SetupCamera(GameObject playerGO)
    {
        GameObject cam = Instantiate(Camera, Vector3.zero, Quaternion.identity, playerGO.transform);

        cam.GetComponent<vThirdPersonCamera>().currentTarget = null;
        cam.GetComponent<vThirdPersonCamera>().target = null;

        cam.GetComponent<vThirdPersonCamera>().currentTarget = Player.transform.GetChild(0);
        cam.GetComponent<vThirdPersonCamera>().target = Player.transform.GetChild(0);

        playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().Setup(cam.GetComponent<vThirdPersonCamera>());
    }

    public void UpdatePlayer(NetEntity player)
    {
        if(player.Online != false)
        {
            NetTransform transform = MakeEntity.GetComponent<NetTransform>(player);

            Vector3 newPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Quaternion newRot = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

            PlayerList[player.ConnectionID].transform.GetChild(0).position = newPos;
            PlayerList[player.ConnectionID].transform.GetChild(0).rotation = newRot;
        }
        else
        {
            Destroy(PlayerList[player.ConnectionID]);
        }
      
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    IEnumerator SendUpdate()
    {
        while (updatePlayer)
        {
            localPlayer = MakeEntity.UpdateTransform(localPlayer, PlayerList[localPlayer.ConnectionID].transform.GetChild(0).transform);

            string json = JsonUtility.ToJson(localPlayer);

            DataSender.SendServerMessage(json);

            yield return new WaitForSeconds(3);
        }
    }
}

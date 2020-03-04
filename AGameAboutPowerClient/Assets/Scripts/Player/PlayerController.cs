using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int MyConnectionID;
    public NetEntity localPlayer;

    public GameObject PlayerPrefab;
    public GameObject Camera;
    private GameObject Player;

    public Transform SpawnPoint;

    public NetworkedTransform NetworkedTransform;
    public NetworkedAnimator NetworkedAnimator;

    public Dictionary<int, GameObject> PlayerList = new Dictionary<int, GameObject>();


    public void HandlePlayer(NetEntity player)
    {
        if (PlayerList.ContainsKey(player.ConnectionID))
        {
            UpdatePlayer(player);
        }
        else
        {
            FDebug.Log.Message("Instansiating player: " + player.ConnectionID);
            InstansiateNewPlayer(player);
        }
    }


    public void InstansiateNewPlayer(NetEntity player)
    {
        GameObject playerGO = Instantiate(PlayerPrefab, SpawnPoint.position, SpawnPoint.rotation);

        if (player.ConnectionID != NetworkManager.instance.ConnectionID)
        {
            playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().isLocalPlayer = false;
        }
        else
        {
            playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().isLocalPlayer = true;
            Player = playerGO;
            SetupCamera(playerGO);
            localPlayer = player;
          
        }

        NetworkedTransform = playerGO.transform.GetChild(0).GetComponent<NetworkedTransform>().Inject(this);
        NetworkedAnimator = playerGO.transform.GetChild(0).GetComponent<NetworkedAnimator>().Inject(this);

        playerGO.name = "Player: " + player.ConnectionID;
        playerGO.GetComponent<PlayerNameSignView>().Inject(player.ConnectionID);


        PlayerList.Add(player.ConnectionID, playerGO);


        NetworkedTransform.updatePlayer = true;
        NetworkedTransform.StartCoroutine("SendTransform");
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
            NetworkedTransform.ReceiveTransform(player);
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

    
}

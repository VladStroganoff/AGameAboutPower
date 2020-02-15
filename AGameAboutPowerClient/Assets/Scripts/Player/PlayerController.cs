using Invector.vCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject PlayerPrefab;
    public GameObject Camera;
    public GameObject Player;

    public void InstansiateNewPlayer(PlayerData player)
    {
        GameObject playerGO = Instantiate(PlayerPrefab);

        if (NetworkManager.instance.localPlayer.ConnectionID != player.ConnectionID)
        {
            playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().isLocalPlayer = false;
        }
        else
        {
            playerGO.transform.GetChild(0).GetComponent<vThirdPersonInput>().isLocalPlayer = true;
            Player = playerGO;
            SetupCamera(playerGO);
        }


        playerGO.name = "Player: " + player.ConnectionID;
        playerGO.GetComponent<PlayerNameSignView>().Inject(player.ConnectionID);


        NetworkManager.instance.PlayerList.Add(player.ConnectionID, playerGO);

        NetworkManager.instance.updatePlayer = true;
        NetworkManager.instance.StartCoroutine("SendUpdate");
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

    public void UpdatePlayer(PlayerData player)
    {
        if(player.Online != false)
        {
            Vector3 newPos = new Vector3(player.position.x, player.position.y, player.position.z);
            Quaternion newRot = new Quaternion(player.rotation.x, player.rotation.y, player.rotation.z, player.rotation.w);

            NetworkManager.instance.PlayerList[player.ConnectionID].transform.GetChild(0).position = newPos;
            NetworkManager.instance.PlayerList[player.ConnectionID].transform.GetChild(0).rotation = newRot;
        }
        else
        {
            Destroy(NetworkManager.instance.PlayerList[player.ConnectionID]);
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

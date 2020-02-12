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
            playerGO.GetComponent<PlayerNetworkController>().Inject(false);
        }
        else
        {
            playerGO.GetComponent<PlayerNetworkController>().Inject(true);
            Player = playerGO;
            GameObject cam = Instantiate(Camera, Vector3.zero, Quaternion.identity, playerGO.transform);

            cam.GetComponent<vThirdPersonCamera>().currentTarget = null;
            cam.GetComponent<vThirdPersonCamera>().target = null;

            cam.GetComponent<vThirdPersonCamera>().currentTarget = Player.transform.GetChild(0);
            cam.GetComponent<vThirdPersonCamera>().target = Player.transform.GetChild(0);
        }


        playerGO.name = "Player: " + player.ConnectionID;
        playerGO.GetComponent<PlayerNameSignView>().Inject(player.ConnectionID);


        NetworkManager.instance.PlayerList.Add(player.ConnectionID, playerGO);

        NetworkManager.instance.updatePlayer = true;
        NetworkManager.instance.StartCoroutine("SendUpdate");
    }

    public void UpdatePlayer(PlayerData player)
    {
        Vector3 newPos = new Vector3(player.position.x, player.position.y, player.position.z);
        Quaternion newRot = new Quaternion(player.rotation.x, player.rotation.y, player.rotation.z, player.rotation.w);

        NetworkManager.instance.PlayerList[player.ConnectionID].transform.GetChild(0).position = newPos;
        NetworkManager.instance.PlayerList[player.ConnectionID].transform.GetChild(0).rotation = newRot;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

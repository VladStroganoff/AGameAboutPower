using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Assets.Scripts;



public class NetworkedTransform : MonoBehaviour
{
    public bool updatePlayer { get; set; }
    public float UpdateIntervals = 3;
    PlayerController playerController;


    public NetworkedTransform Inject(PlayerController controller)
    {
        playerController = controller;
        return this;
    }


    IEnumerator SendTransform()
    {
        while (updatePlayer)
        {
            playerController.localPlayer = MakeEntity.UpdateTransform(playerController.localPlayer, transform);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string json = JsonConvert.SerializeObject(playerController.localPlayer, settings);

            DataSender.SendServerMessage(json);

            yield return new WaitForSeconds(UpdateIntervals);
        }
    }

    public void ReceiveTransform(NetEntity player)
    {
        NetTransform netTransform = MakeEntity.GetComponent<NetTransform>(player);

        Vector3 newPos = new Vector3(netTransform.position.x, netTransform.position.y, netTransform.position.z);
        Quaternion newRot = new Quaternion(netTransform.rotation.x, netTransform.rotation.y, netTransform.rotation.z, netTransform.rotation.w);

        if (Vector3.Distance(newPos, playerController.PlayerList[player.ConnectionID].transform.GetChild(0).position) > 0.1f)
            transform.position = newPos;

        if (Quaternion.Angle(newRot, playerController.PlayerList[player.ConnectionID].transform.GetChild(0).rotation) > 0.1f)
            transform.rotation = newRot;
    }
}

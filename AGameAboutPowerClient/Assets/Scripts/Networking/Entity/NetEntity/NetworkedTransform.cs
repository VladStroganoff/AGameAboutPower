using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class NetworkedTransform : MonoBehaviour
{
    public bool updatePlayer { get; set; }
    public float UpdateIntervals = 3;
    WorldController playerController;
    NetEntity myEntity;



    public void Inject(WorldController controller, NetEntity netEnt)
    {
        playerController = controller;
        playerController.UpdateNetEnts += ReceiveTransform;
        myEntity = netEnt;
        updatePlayer = true;
        StartCoroutine("SendTransform");
    }


    IEnumerator SendTransform()
    {
        while (updatePlayer)
        {

            myEntity = MakeEntity.UpdateTransform(myEntity, transform);

            DataSender.SendServerMessage(myEntity);

            yield return new WaitForSeconds(UpdateIntervals);
        }
    }

    public void ReceiveTransform(NetEntity player)
    {
        if (player.ConnectionID != myEntity.ConnectionID)
            return;

        NetTransform netTransform = MakeEntity.GetComponent<NetTransform>(player);

        Vector3 newPos = new Vector3(netTransform.position.x, netTransform.position.y, netTransform.position.z);
        Quaternion newRot = new Quaternion(netTransform.rotation.x, netTransform.rotation.y, netTransform.rotation.z, netTransform.rotation.w);

        if (Vector3.Distance(newPos, playerController.PlayerList[player.ConnectionID].transform.GetChild(0).position) > 0.1f)
            transform.position = newPos;

        if (Quaternion.Angle(newRot, playerController.PlayerList[player.ConnectionID].transform.GetChild(0).rotation) > 0.1f)
            transform.rotation = newRot;
    }

    private void OnDisable()
    {
        playerController.UpdateNetEnts -= ReceiveTransform;
    }
}

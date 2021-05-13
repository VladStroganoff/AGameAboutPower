using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    Dictionary<int, DressController> _dressControllers = new Dictionary<int, DressController>();

    public void AddPlayer(Player player)
    {
        player.PlayerDisconnect += RemovePlayer;
        _dressControllers.Add(player.ID, player.GetComponent<DressController>());
    }

    public void RemovePlayer(Player player)
    {
        _dressControllers.Remove(player.ID);
    }

    public void ChangeWear(Netwearable netWear, int id)
    {
        Wearable wear = new Wearable();
        wear.Initialize(netWear);
        RuntimeItem runItem = LoadController.instance.LoadRuntimeItem(wear, _dressControllers[id]);
        ServerSend.ChangeWearable(id, netWear);
    }
}

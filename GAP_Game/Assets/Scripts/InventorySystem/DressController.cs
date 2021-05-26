using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSpawned
{
    public PlayerManager player;
}


public class DressController : MonoBehaviour, IDressController
{
    public Dictionary<string, GameObject> Wear = new Dictionary<string, GameObject>();
    public InventoryModel Inventory;

    public Transform Rigg;
    BoneCombiner _boneCombine;
    ILoadController _loadControl;
    int _playerID;


    [Inject]
    public void Inject(IInventoryView inventorView, ILoadController loadControl)
    {
        Wear = inventorView.GetWearSlots();
        _boneCombine = new BoneCombiner(Rigg);
        _loadControl = loadControl;
    }


    public void EquipItem(Wearable wearableItem)
    {
    }

    public void AddLoadedWear(RuntimeItem runItem)
    {
        if (Wear.ContainsKey(runItem.Item.Slot))
        {
            Destroy(Wear[runItem.Item.Slot]);
            Wear[runItem.Item.Slot] = runItem.Prefab;
            Wear[runItem.Item.Slot] = _boneCombine.AddLimb(runItem.Prefab).gameObject;
        }
    }

    public void SendSwapRequest(RuntimeItem newWear, RuntimeItem oldWear)
    {
        Debug.Log("Send swap wear request to server: " + newWear.Item.Name);
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        Netwearable netItem = newWear.Item.MakeNetWear();
        Netwearable netItemOld = oldWear.Item.MakeNetWear();
        string newWearjson = JsonConvert.SerializeObject(netItem, settings);
        string oldWearjson = JsonConvert.SerializeObject(netItemOld, settings);
        ClientSend.SendJsonPackage(newWearjson);
        ClientSend.SendJsonPackage(oldWearjson);
    }

    public void RecieveSwapWear(RuntimeItem runItem)
    {
        Debug.Log(runItem.Item.Name);
        if (Wear.ContainsKey(runItem.Item.Slot))
        {
            if (Wear[runItem.Item.Slot] != null)
                GameObject.Destroy(Wear[runItem.Item.Slot].gameObject);

            Wear[runItem.Item.Slot] = runItem.Prefab;
            Wear[runItem.Item.Slot] = _boneCombine.AddLimb(runItem.Prefab).gameObject;
        }
        else
        {
            Debug.Log($"Itemslot missmatch Item: {runItem.Item.Name}, Slot: {runItem.Item.Slot}");
        }
    }

    public void InitializePlayer(Dictionary<string, Item> items, int playerID)
    {
        _playerID = playerID;

        foreach (var pair in items)
        {
            RuntimeItem runtimeItem = _loadControl.LoadRuntimeItem(pair.Value, _playerID);
        }
    }


}

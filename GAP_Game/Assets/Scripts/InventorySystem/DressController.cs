using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerDresserSpawned
{
    public DressController DressController;
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
    public void Inject(SignalBus _signalBus, IInventoryView inventorView, ILoadController loadControl)
    {
        _signalBus.Fire(new PlayerDresserSpawned() { DressController = this });
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

    public void SendSwapRequest(RuntimeItem runItem)
    {
        Debug.Log("Send swap wear request to server: " + runItem.Item.Name);
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        Netwearable netItem = runItem.Item.MakeNetWear();
        string json = JsonConvert.SerializeObject(netItem, settings);
        ClientSend.SendJsonPackage(json);
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

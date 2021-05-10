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
    public List<RuntimeItem> RuntimeItems = new List<RuntimeItem>();

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
        _signalBus.Subscribe<ItemLoadedSignal>(ItemLoaded);
    }


    public void EquipItem(Wearable wearableItem)
    {
    }

    public void AddWear(RuntimeItem runItem)
    {
        if (Wear.ContainsKey(runItem.Item.Slot))
        {
            Wear[runItem.Item.Slot] = runItem.Prefab;
            Wear[runItem.Item.Slot] = _boneCombine.AddLimb(runItem.Prefab).gameObject;
        }
        else
        {
            GameObject.Destroy(Wear[runItem.Item.Name]);
        }
    }

    public void SwapWear(RuntimeItem runItem)
    {
        Debug.Log(runItem.Item.Name);
        if (Wear.ContainsKey(runItem.Item.Slot))
        {
            if(Wear[runItem.Item.Slot] != null)
                GameObject.Destroy(Wear[runItem.Item.Slot].gameObject);

            Wear[runItem.Item.Slot] = runItem.Prefab;
            Wear[runItem.Item.Slot] = _boneCombine.AddLimb(runItem.Prefab).gameObject;
        }
        else
        {
            Debug.Log($"Itemslot missmatch Item: {runItem.Item.Name}, Slot: {runItem.Item.Slot}");
        }
    }

    public void InitializeOtherPlayer(Dictionary<string, Item> items, int playerID)
    {
        _playerID = playerID;

        foreach (var pair in items)
        {
            if (pair.Value.Slot == "Head_Slot" || pair.Value.Slot == "Torso_Slot" || pair.Value.Slot == "Legs_Slot" ||
           pair.Value.Slot == "Right_Arm_Slot" || pair.Value.Slot == "Left_Arm_Slot")
            {
                RuntimeItem runtimeItem = _loadControl.LoadRuntimeItem(pair.Value, _playerID);
            }
        }
    }

    public void ItemLoaded(ItemLoadedSignal loadedRuntime) // only for displaying non local players gear
    {
        if (loadedRuntime.PlayerID != _playerID)
            return;

        AddWear(loadedRuntime.LoadedItem);
    }
}

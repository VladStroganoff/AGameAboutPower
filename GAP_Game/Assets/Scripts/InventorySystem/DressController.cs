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


    [Inject]
    public void Inject(SignalBus _signalBus, IInventoryView inventorView)
    {
        _signalBus.Fire(new PlayerDresserSpawned() { DressController = this });
        Wear = inventorView.GetWearSlots();
        _boneCombine = new BoneCombiner(Rigg);
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
}

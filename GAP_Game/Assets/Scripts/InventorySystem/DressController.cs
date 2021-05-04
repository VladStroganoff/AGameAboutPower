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
            Wear.Add(runItem.Item.Name, runItem.Prefab);
            _boneCombine.AddLimb(runItem.Prefab);
        }
        else
        {
            GameObject.Destroy(Wear[runItem.Item.Name]);
        }
    }
}

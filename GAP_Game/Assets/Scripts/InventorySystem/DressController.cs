using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;




public class DressController : MonoBehaviour, IDressController
{
    public List<Wearable> StartWear = new List<Wearable>();
    public Dictionary<string, GameObject> Wear = new Dictionary<string, GameObject>();
    public List<RuntimeItem> RuntimeItems = new List<RuntimeItem>();

    public Transform Rigg;
    BoneCombiner _boneCombine;
    ILoadController _loadControl;


    [Inject]
    public void Inject(SignalBus bus, ILoadController loadControl)
    {
        bus.Subscribe<ItemLoadedSignal>(WearLoaded);
        _loadControl = loadControl;
    }

    void Start()
    {
        _boneCombine = new BoneCombiner(Rigg);
        foreach (Wearable item in StartWear)
        {
            RuntimeItems.Add(_loadControl.LoadRuntimeItem(item));
        }
    }

    public void WearLoaded(ItemLoadedSignal runItem)
    {
            _boneCombine.AddLimb(runItem.LoadedItem.Prefab);
    }
    public void AddWear(RuntimeItem runItem)
    {
        if(!Wear.ContainsKey(runItem.Item.Name))
            Wear.Add(runItem.Item.Name, runItem.Prefab);
        else
        {
            GameObject.Destroy(Wear[runItem.Item.Name]);
            Wear.Remove(runItem.Item.Name);
        }
        _boneCombine.AddLimb(runItem.Prefab);
    }

    public void EquipItem(Wearable wearableItem)
    {
    }
}

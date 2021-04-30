using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;




public class DressController : MonoBehaviour, IDressController
{
    public List<Wearable> StartWear = new List<Wearable>();
    public List<GameObject> Wear = new List<GameObject>();
    public List<RuntimeItem> RuntimeItems = new List<RuntimeItem>();

    public Transform Rigg;
    BoneCombiner _boneCombine;
    ILoadController _loadControl;


    [Inject]
    public void Inject(SignalBus bus, ILoadController loadControl)
    {
        bus.Subscribe<WearableLoadedSignal>(AddWear);
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

    public void AddWear(WearableLoadedSignal runItem)
    {
            _boneCombine.AddLimb(runItem.RuntimeWear.Prefab);
    }

    public void EquipItem(Wearable wearableItem)
    {
    }
}

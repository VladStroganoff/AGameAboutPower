using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DressController : MonoBehaviour, IDressController
{
    public List<Wearable> StartWear = new List<Wearable>();
    public Transform Rigg;
    BoneCombiner _boneCombine;
    ILoadController _loadControl;

    [Inject]
    public void Inject(ILoadController loadControl)
    {
        _loadControl = loadControl;
    }

    void Start()
    {
        _boneCombine = new BoneCombiner(Rigg);
        foreach (Wearable item in StartWear)
        {
            _boneCombine.AddLimb(_loadControl.LoadWearable(item.Prefab));
        }
    }

    public void EquipItem(Wearable wearableItem)
    {
    }
}

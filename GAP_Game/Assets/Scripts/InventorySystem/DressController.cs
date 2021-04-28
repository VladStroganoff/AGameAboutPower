using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class DressController : MonoBehaviour, IDressController
{
    public List<Wearable> StartWear = new List<Wearable>();
    public List<GameObject> Wear = new List<GameObject>();
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
            //_boneCombine.AddLimb(item.Prefab, item._boneNames);
        }
    }

    IEnumerator LoadItems()
    {
        foreach (Wearable item in StartWear)
        {
            //GameObject wear = _loadControl.LoadWearable(item.Prefab);
            //_boneCombine.AddLimb(item.Prefab, item._boneNames);
        }
        yield return null;
    }

    public void EquipItem(Wearable wearableItem)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DressController : MonoBehaviour
{
    public Dictionary<string, GameObject> Wear = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> RuntimeItems = new Dictionary<string, GameObject>();

    public Transform Rigg;
    BoneCombiner _boneCombine;

    void Start()
    {
        LoadController.instance.loadedWear += EquipItem;
        _boneCombine = new BoneCombiner(Rigg);
    }

    public void InitializeWear(JsonItemDictionary playerSaveData)
    {
        foreach (var wear in playerSaveData.Wearables)
        {
            var runTime = LoadController.instance.LoadRuntimeItem(wear);
        }
    }

    public void EquipItem(RuntimeItem runTime)
    {
        Wear.Add(runTime.Item.Slot, runTime.Prefab);
        RuntimeItems.Add(runTime.Item.Slot, runTime.Prefab);

        if (runTime.Item.Slot == "Head_Slot" || runTime.Item.Slot == "Torso_Slot" || runTime.Item.Slot == "Legs_Slot" ||
            runTime.Item.Slot == "Right_Arm_Slot" || runTime.Item.Slot == "Left_Arm_Slot")
        {
            AddWear(runTime);
        }
    }

    public void WearLoaded(RuntimeItem item)
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
}

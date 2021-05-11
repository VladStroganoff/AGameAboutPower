using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DressController : MonoBehaviour
{
    public Dictionary<string, GameObject> Wear = new Dictionary<string, GameObject>();

    public Transform Rigg;
    BoneCombiner _boneCombine;

    void Start()
    {
        _boneCombine = new BoneCombiner(Rigg);
    }

    public void InitializeWear(JsonItemDictionary playerSaveData)
    {
        foreach (var wear in playerSaveData.Wearables)
        {
            var runTime = LoadController.instance.LoadRuntimeItem(wear, this);
        }
    }

    public void EquipItem(RuntimeItem runTime)
    {
      
    }

    public void WearLoaded(RuntimeItem item)
    {

    }

    public void AddWear(RuntimeItem runItem)
    {

        if (runItem.Item.Slot == "Head_Slot" || runItem.Item.Slot == "Torso_Slot" || runItem.Item.Slot == "Legs_Slot" ||
            runItem.Item.Slot == "Right_Arm_Slot" || runItem.Item.Slot == "Left_Arm_Slot")
        {
            if (!Wear.ContainsKey(runItem.Item.Slot))
                Wear.Add(runItem.Item.Slot, _boneCombine.AddLimb(runItem.Prefab).gameObject);
            else
                Debug.Log($"{gameObject.name} already contains wear: {runItem.Item.Slot}");
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class DressController : MonoBehaviour
{
    Dictionary<string, GameObject> Wear = new Dictionary<string, GameObject>();

    public InventoryModel Inventory;
    public Transform Rigg;
    BoneCombiner _boneCombine;

    void Start()
    {
        _boneCombine = new BoneCombiner(Rigg);
    }

    public void InitializeWear(NetInventory playerSaveData)
    {
        List<Wearable> wearables = playerSaveData.Wearables.ToList();
        List<Item> asItems = new List<Item>();
        foreach (var wearable in wearables)
            asItems.Add(wearable as Item);

        LoadController.instance.LoadRuntimeItems(asItems, RunItemsLoaded);
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


    public int RunItemsLoaded(List<RuntimeItem> runtimeItems)
    {
        foreach (var runItem in runtimeItems)
        {
            Inventory.AddItemToPlayer(runItem);

            if (runItem.Item.Slot == "Head_Slot" || runItem.Item.Slot == "Torso_Slot" || runItem.Item.Slot == "Legs_Slot" ||
                runItem.Item.Slot == "Right_Arm_Slot" || runItem.Item.Slot == "Left_Arm_Slot")
            {
                if (!Wear.ContainsKey(runItem.Item.Slot))
                    Wear.Add(runItem.Item.Slot, _boneCombine.AddLimb(runItem.Prefab).gameObject);
                else
                {
                    Destroy(Wear[runItem.Item.Slot]);
                    Wear[runItem.Item.Slot] = _boneCombine.AddLimb(runItem.Prefab).gameObject;
                }
            }
        }
        return 1;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryModel : MonoBehaviour
{
    public int ID;
    public Dictionary<string, RuntimeItem> Inventory { get; private set; } = new Dictionary<string, RuntimeItem>();
    public List<RuntimeItem> InventoryDisplay = new List<RuntimeItem>();


    [Inject]
    public void Inject(SignalBus bus)
    {
        bus.Subscribe<ItemLoadedSignal>(ItemLoaded);
    }
    public void LoadItem(Item item)
    {
        LoadController.instance.LoadRuntimeItem(item, ID);
    }

    public void AddSlot(string slot)
    {
        Inventory.Add(slot, null);
    }
    public void ItemLoaded(ItemLoadedSignal loadedRuntime)
    {
        if (loadedRuntime.PlayerID != ID)
            return;

        if (!Inventory.ContainsKey(loadedRuntime.LoadedItem.Item.Slot))
           Inventory.Add(loadedRuntime.LoadedItem.Item.Slot, loadedRuntime.LoadedItem);
        else
            Inventory[loadedRuntime.LoadedItem.Item.Slot] = loadedRuntime.LoadedItem;

        GetComponent<DressController>().AddLoadedWear(loadedRuntime.LoadedItem);
    }
}

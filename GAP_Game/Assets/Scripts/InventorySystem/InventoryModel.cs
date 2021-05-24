using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryModel : MonoBehaviour
{
    public int ID;
    public Dictionary<string, RuntimeItem> Inventory { get; private set; } = new Dictionary<string, RuntimeItem>();

    [Inject]
    public void Inject(SignalBus bus)
    {
        bus.Subscribe<ItemLoadedSignal>(ItemLoaded);
    }
    public void LoadItem(Item item)
    {
        LoadController.instance.LoadRuntimeItem(item, ID);
    }

    public NetInventory MakeNetCopy()
    {
        NetInventory netCopy = new NetInventory();

        foreach(var item in Inventory)
        {
            if (item.Value.Item is Wearable)
            {
                for (int i = 0; i < netCopy.Wearables.Length; i++)
                {
                    if (netCopy.Wearables[i].Slot == item.Value.Item.Slot)
                        netCopy.Wearables[i] = item.Value.Item as Wearable;
                }
            }
            if (item.Value.Item is Holdable)
            {
                for (int i = 0; i < netCopy.Wearables.Length; i++)
                {
                    if (netCopy.Holdables[i].Slot == item.Value.Item.Slot)
                        netCopy.Holdables[i] = item.Value.Item as Holdable;
                }
            }
            if (item.Value.Item is Consumable)
            {
                for (int i = 0; i < netCopy.Wearables.Length; i++)
                {
                    if (netCopy.Consumable[i].Slot == item.Value.Item.Slot)
                        netCopy.Consumable[i] = item.Value.Item as Consumable;
                }
            }
            if (item.Value.Item is Misc)
            {
                for (int i = 0; i < netCopy.Wearables.Length; i++)
                {
                    if (netCopy.Misc[i].Slot == item.Value.Item.Slot)
                        netCopy.Misc[i] = item.Value.Item as Misc;
                }
            }
        }
        return netCopy;
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

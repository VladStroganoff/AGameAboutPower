using System.Collections.Generic;
using UnityEngine;


public class NetLoot : NetEntity
{
    public List<Item> GetItems()
    {
        List<Item> items = new List<Item>();

        foreach (var item in Items)
        {
            if (item is Netwearable)
            {
                Netwearable netWear = item as Netwearable;
                Wearable wearable = new Wearable();
                wearable.Initialize(netWear);
                items.Add(wearable);

            }
            if (item is NetHoldable)
            {
                NetHoldable netHold = item as NetHoldable;
                Holdable holdable = new Holdable();
                holdable.Initialize(netHold);
                items.Add(holdable);
            }
            if (item is NetConsumable)
            {
                NetConsumable netCons = item as NetConsumable;
                Consumable consumable = new Consumable();
                consumable.Initialize(netCons);
                items.Add(consumable);
            }
            if (item is NetMisc)
            {
                NetMisc netMisc = item as NetMisc;
                Misc misc = new Misc();
                misc.Initialize(netMisc);
                items.Add(misc);
            }
        }

        return items;
    }
    public int lootID;
    public int ownerID;
    public NetItem[] Items;
    public Vector3 Position;
    public Quaternion Rotation;
}
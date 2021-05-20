using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour, ILootController
{
    public Dictionary<int, LootView> Loot = new Dictionary<int, LootView>(); // for destroying when picked up

    public void SpawnLoot(NetLoot netLoot)
    {
        GameObject lootGO = new GameObject();
        LootView lootView = lootGO.AddComponent<LootView>();
        lootView.Initialize(ConvertNetItems(netLoot.Items));
        lootGO.transform.position = netLoot.Position;
        lootGO.transform.rotation = netLoot.Rotation;
        Loot.Add(netLoot.ID, lootView);
    }

    List<Item> ConvertNetItems(NetItem[] netItems)
    {
        List<Item> items = new List<Item>();

        foreach(var item in netItems)
        {
            if(item is Netwearable)
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
}

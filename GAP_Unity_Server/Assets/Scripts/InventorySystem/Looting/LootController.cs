using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour
{
    public bool testSveLoot;

    public DatabaseController DatabaseControl;
    public List<LootSpawner> LootSpawners = new List<LootSpawner>();
    public List<LootItemView> Loot = new List<LootItemView>();
    public int MinItems;
    public int MaxItems;

    private void Start()
    {
        DatabaseControl.databaseLoaded += InitializeLoot;
    }

    public void AddLootSpawner(LootSpawner spawner)
    {
        LootSpawners.Add(spawner);
    }

    public void InitializeLoot()
    {
        foreach(var spawner in LootSpawners)
        {
            switch (spawner.Type)
            {
                case LootSpawnType.Ammunition:
                    continue;
                case LootSpawnType.Food:
                    continue;
                case LootSpawnType.Player:
                    continue;
                case LootSpawnType.Random:
                    SpawnRandomLoot(spawner);
                    continue;
                case LootSpawnType.Rare:
                    continue;
                case LootSpawnType.Weapons:
                    continue;
                case LootSpawnType.Wearables:
                    continue;
            }
        }
    }

    void SpawnRandomLoot(LootSpawner spawner)
    {
        List<Item> allItems = DatabaseControl.GetAllItems();
        int randNumb = Random.Range(MinItems, MaxItems);
        List<Item> randItem = new List<Item>();
        for (int i = 0; i < randNumb; i++)
        {
            randItem.Add(allItems[Random.RandomRange(0, allItems.Count - 1)]);
        }
        spawner.SpawnLoot(randItem);
    }

    public void LootUpdatePos(LootItemView view)
    {
        Loot.Add(view);
    }

    public void LootPickedUp(List<RuntimeItem> items, int id)
    {
        NetLootItem netLoot = new NetLootItem();
        List<NetItem> netItems = new List<NetItem>();
        foreach (var item in items)
        {
            if (item is Holdable)
            {
                var netItem = item.Item.MakeNetWear();
                netItems.Add(netItem);
            }
            if (item is Wearable)
            {
                var netItem = item.Item.MakeNetWear();
                netItems.Add(netItem);
            }
            if (item is Consumable)
            {
                var netItem = item.Item.MakeNetConsumable();
                netItems.Add(netItem);
            }
            if (item is Misc)
            {
                var netItem = item.Item.MakeNetMisc();
                netItems.Add(netItem);
            }
        }
        netLoot.ID = id;
        netLoot.Items = netItems.ToArray();
        netLoot.Position = transform.position;
        netLoot.Rotation = transform.rotation;
    }
}

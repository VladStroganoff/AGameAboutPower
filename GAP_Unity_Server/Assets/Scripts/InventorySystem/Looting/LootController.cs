using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour
{
    public bool testSveLoot;

    public DatabaseController DatabaseControl;
    public List<LootSpawner> LootSpawners = new List<LootSpawner>();
    public List<LootView> Loot = new List<LootView>();
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

    public void LootUpdatePos(LootView view)
    {
        Loot.Add(view);
        DatabaseControl.AddLoot(view);
    }

 

    public void LootPickedUp(LootView view, int id)
    {
        Debug.Log($"Player-{id} picked up Item {view.gameObject.name}");
        NetLoot netLoot = new NetLoot();
        netLoot.Position = view.transform.position;
        netLoot.Rotation = view.transform.rotation;
        List<NetItem> netItems = new List<NetItem>();
        foreach (var item in view.Items)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour
{
    public bool testSveLoot;

    public DatabaseController DatabaseControl;
    public List<LootSpawner> LootSpawners = new List<LootSpawner>();
    public int MinItems;
    public int MaxItems;


    public void AddLootSpawner(LootSpawner spawner)
    {
        LootSpawners.Add(spawner);
        AddLoot(spawner);
    }

    public void AddLoot(LootSpawner spawner)
    {
        switch (spawner.Type)
        {
            case LootSpawnType.Ammunition:
                return;
            case LootSpawnType.Food:
                return;
            case LootSpawnType.Player:
                return;
            case LootSpawnType.Random:
                SpawnRandomLoot(spawner);
                return;
            case LootSpawnType.Rare:
                return;
            case LootSpawnType.Weapons:
                return;
            case LootSpawnType.Wearables:
                return;
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

    void InitializeLoot() // Maybe some day spawn loot only around Player
    {
       
    }



    public void LootPickedUp(NetLootItem items)
    {

    }
}

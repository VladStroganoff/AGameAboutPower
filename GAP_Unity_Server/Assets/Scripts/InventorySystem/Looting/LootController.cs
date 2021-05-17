using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour
{
    public bool testSveLoot;

    public DatabaseController DatabaseControl;
    public List<LootSpawner> LootSpawners = new List<LootSpawner>();


    public void AddLootSpawner(LootSpawner spawner)
    {
        LootSpawners.Add(spawner);
    }

    void InstansiateLoot(LootSpawner spawner, List<Item> items) // Maybe some day spawn loot only around Player
    {
        //foreach(var spawner in LootSpawners)
        //{

        //}
    }

    public void LootPickedUp(NetLootItem items)
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour
{
    public bool testSveLoot;

    public DatabaseController DatabaseControl;
    public List<LootSpawner> LootSpawners = new List<LootSpawner>();
    public Dictionary<int, LootView> Loot = new Dictionary<int, LootView>();
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
        Loot.Add(view.ID, view);
        DatabaseControl.AddLoot(view);
    }

    public void UpdateLoot(NetLoot loot)
    {
        NetLoot[] updatedLoot = new NetLoot[1] { loot };
        UpdateLootForPlayers(updatedLoot);

        if (loot.Items.Length < 1)
        {
            DestroyLoot(loot);
            return;
        }

        ReActivateLoot(loot);
    }

    void UpdateLootForPlayers(NetLoot[] updatedLoot)
    {
        foreach (Client client in Server.clients.Values)
        {
            if (client.player != null)
            {
                ServerSend.SpawnLoot(client.id, updatedLoot);
            }
        }
    }
    void ReActivateLoot(NetLoot loot)
    {
        Loot[loot.lootID].UpdateLoot(loot);
        Loot[loot.lootID].GetComponent<BoxCollider>().enabled = true;
    }

    private void DestroyLoot(NetLoot loot)
    {
        DatabaseControl.RemoveLoot(Loot[loot.lootID].gameObject.GetComponent<LootView>().ID);
        Destroy(Loot[loot.lootID].gameObject);
        Loot.Remove(loot.lootID);
    }

    public void LookAtLoot(LootView view, int playerID)
    {
        NetLoot loot = DatabaseControl.GetLoot(view.ID);
        loot.ownerID = playerID;
        ServerSend.LookAtLoot(playerID, loot);
        loot.ownerID = -1;
    }
}

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public delegate void DataDaseLoaded();

public class DatabaseController : MonoBehaviour
{
    List<Item> _allAvalableItems = new List<Item>();
    public static DatabaseModel _database;
    JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
    public DataDaseLoaded databaseLoaded;
    Dictionary<int, NetLoot> _loot = new Dictionary<int, NetLoot>();

private void Start()
    {
        LoadWorldState();
        //SaveNewAvalableItems();
    }

    void SaveNewAvalableItems()
    {
        _database = new DatabaseModel();

        var AllItemIbjects = Resources.LoadAll("Items", typeof(ScriptableObject));
        foreach(var itemobject in AllItemIbjects)
        {
            if(itemobject is ScriptableObject)
            {
                if(itemobject is Item)
                {
                    _allAvalableItems.Add(itemobject as Item);
                }
            }
        }
        _database.AllItems = SortAndSerialize(_allAvalableItems);
        SaveDatabade();
    }

    public List<Item> GetAllItems()
    {
        return _allAvalableItems;
    }

    

    public List<Wearable> GetAllWear()
    {
        return _database.AllItems.Wearables.ToList();
    }
    public List<Holdable> GetAllHoldables()
    {
        return _database.AllItems.Holdables.ToList();
    }

    public List<Consumable> GetAllConsumables()
    {
        return _database.AllItems.Consumable.ToList();
    }

    public List<Misc> GetAllMisc()
    {
        return _database.AllItems.Misc.ToList();
    }

    public void LoadWorldState()
    {
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/GAPServerDatabase.json");
        DatabaseModel database = JsonConvert.DeserializeObject(jsonString, _settings) as DatabaseModel;
        _allAvalableItems.AddRange(database.AllItems.Consumable);
        _allAvalableItems.AddRange(database.AllItems.Wearables);
        _allAvalableItems.AddRange(database.AllItems.Holdables);
        _allAvalableItems.AddRange(database.AllItems.Misc);
        _database = database;
        databaseLoaded.Invoke(); // this event tells everybody that the database is loaded
    }

    public void AddLoot(LootView lootView)
    {
        NetLoot netLoot = new NetLoot(lootView);
        AddLoot(netLoot);
    }
    void AddLoot(NetLoot loot)
    {
        if (_database.WorldItems == null)
            _database.WorldItems = new JsonWorldLoot();

        List<NetLoot> worldLoot = new List<NetLoot>();
        if (_database.WorldItems.Loot != null)
            worldLoot = _database.WorldItems.Loot.ToList();

        worldLoot.Add(loot);
        _database.WorldItems.Loot = worldLoot.ToArray(); // very very ugly baby! this should be a database in the future.
        _loot.Add(loot.lootID, loot);
    }

    NetInventory SortAndSerialize(List<Item> items)
    {
        NetInventory playerSaveData = new NetInventory();
        List<Wearable> wearables = new List<Wearable>();
        List<Holdable> holdables = new List<Holdable>();
        List<Consumable> consumables = new List<Consumable>();
        List<Misc> miscs = new List<Misc>();

        foreach (var item in items)
        {
            if (item is Wearable)
            {
                wearables.Add(item as Wearable);
            }
            if (item is Holdable)
            {
                holdables.Add(item as Holdable);
            }
            if (item is Consumable)
            {
                consumables.Add(item as Consumable);
            }
            if (item is Misc)
            {
                miscs.Add(item as Misc);
            }
        }

        playerSaveData.Wearables = wearables.ToArray();
        playerSaveData.Holdables = holdables.ToArray();
        playerSaveData.Consumable = consumables.ToArray();
        playerSaveData.Misc = miscs.ToArray();

       
        return playerSaveData;
    }

    public NetLoot GetLoot(int id)
    {
        return _loot[id];
    }

    public void RemoveLoot(int id)
    {
        _loot.Remove(id);

        List<NetLoot> oldWorldLoot = new List<NetLoot>();
        List<NetLoot> newWorldLoot = new List<NetLoot>();
        if (_database.WorldItems.Loot != null)
            oldWorldLoot = _database.WorldItems.Loot.ToList();

        for(int i = 0; i < oldWorldLoot.Count; i++)
        {
            if (oldWorldLoot[i].lootID != id)
                newWorldLoot.Add(oldWorldLoot[i]);
        }    

        _database.WorldItems.Loot = oldWorldLoot.ToArray(); // very very ugly baby! this should be a database in the future.
    }

    void SaveDatabade()
    {
        string json = JsonConvert.SerializeObject(_database, _settings);
        File.WriteAllText(Application.persistentDataPath + "/GAPServerDatabase.json", json);
        Debug.Log($"Saved inventory to: {Application.persistentDataPath}/GAPServerDatabase.json");
    }

}

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

private void Start()
    {
        LoadWorldState();
        //SaveNewAvalableItems();
    }

    void SaveNewAvalableItems()
    {
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
        NetLoot netLoot = new NetLoot();
        netLoot.ID = 1-lootView.gameObject.GetInstanceID();
        netLoot.Position = lootView.transform.position;
        netLoot.Rotation = lootView.transform.rotation;
        List<NetItem> netItems = new List<NetItem>();


        foreach (var runItem in lootView.Items)
        {
            if (runItem.Item is Wearable)
            {
                netItems.Add(runItem.Item.MakeNetWear());
            }
            if (runItem.Item is Holdable)
            {
                netItems.Add(runItem.Item.MakeNetHoldable());
            }
            if (runItem.Item is Consumable)
            {
                netItems.Add(runItem.Item.MakeNetConsumable());
            }
            if (runItem.Item is Misc)
            {
                netItems.Add(runItem.Item.MakeNetMisc());
            }
        }
        netLoot.Items = netItems.ToArray();
        SaveLoot(netLoot);
    }
    void SaveLoot(NetLoot loot)
    {
        List<NetLoot> worldLoot = new List<NetLoot>();
        if (_database.WorldItems.Loot != null)
            worldLoot = _database.WorldItems.Loot.ToList();

        worldLoot.Add(loot);
        _database.WorldItems.Loot = worldLoot.ToArray(); // very very ugly baby!
    }

    JsonItemDictionary SortAndSerialize(List<Item> items)
    {
        JsonItemDictionary playerSaveData = new JsonItemDictionary();
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

    void SaveDatabade()
    {
        string json = JsonConvert.SerializeObject(_database, _settings);
        File.WriteAllText(Application.persistentDataPath + "/GAPServerDatabase.json", json);
        Debug.Log($"Saved inventory to: {Application.persistentDataPath}/GAPServerDatabase.json");
    }

}

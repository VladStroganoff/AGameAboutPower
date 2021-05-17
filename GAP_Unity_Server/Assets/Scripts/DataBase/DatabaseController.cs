using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatabaseController : MonoBehaviour
{
    List<Item> _allAvalableItems = new List<Item>();
    public DatabaseModel _database;
    JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore};


private void Start()
    {
        //SaveScriptableObjectItems();
        //LoadWorldState();
    }

    void CollectAllAvalableItems()
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
    }

    public void LoadWorldState()
    {
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/GAPServerDatabase.json");
        DatabaseModel database = JsonConvert.DeserializeObject(jsonString, _settings) as DatabaseModel;
        _database = database;
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
                Wearable wearable = ScriptableObject.CreateInstance(typeof(Wearable)) as Wearable;
                wearables.Add(wearable);
            }
            if (item is Holdable)
            {
                Holdable holdable = ScriptableObject.CreateInstance(typeof(Holdable)) as Holdable;
                holdables.Add(holdable);
            }
            if (item is Consumable)
            {
                Consumable consumable = ScriptableObject.CreateInstance(typeof(Consumable)) as Consumable;
                consumables.Add(consumable);
            }
            if (item is Misc)
            {
                Misc misc = ScriptableObject.CreateInstance(typeof(Misc)) as Misc;
                miscs.Add(misc);
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
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        string json = JsonConvert.SerializeObject(_database, settings);
        File.WriteAllText(Application.persistentDataPath + "/GAPServerDatabase.json", json);
        Debug.Log($"Saved inventory to: {Application.persistentDataPath}/GAPServerDatabase.json");
    }

}

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

public class ItemLoadedSignal
{
    public int PlayerID;
    public RuntimeItem LoadedItem;
}

public class LoadController : MonoBehaviour, ILoadController
{
    public static LoadController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    string localSavePath = "";
    SignalBus _signalBus;
   
    [Inject]
    public void Inject(SignalBus bus)
    {
        _signalBus = bus;
    }


    public Dictionary<string, Item> LoadInventory(string playerData)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        NetInventory playerSaveData = JsonConvert.DeserializeObject(playerData, settings) as NetInventory;

        Dictionary<string, Item> runTimeDictionary = new Dictionary<string, Item>();

        if(playerSaveData.Wearables != null)
        {
            foreach (var wearable in playerSaveData.Wearables)
            {
                if (!runTimeDictionary.ContainsKey(wearable.Slot))
                {
                    runTimeDictionary.Add(wearable.Slot, wearable as Item);
                }
                else
                    Debug.Log($"Inventory already contains Item {runTimeDictionary[wearable.Slot].Name} on slot: {wearable.Slot}. Trying to also add item: {wearable.Name} to the same slot.");
            }
        }
        if(playerSaveData.Holdables != null)
        {
            foreach (var holdable in playerSaveData.Holdables)
            {
                if (!runTimeDictionary.ContainsKey(holdable.Slot))
                {
                    runTimeDictionary.Add(holdable.Slot, holdable as Item);
                }
                else
                    Debug.Log($"Inventory already contains Item {runTimeDictionary[holdable.Slot].Name} on slot: {holdable.Slot}. Trying to also add item: {holdable.Name} to the same slot.");
            }
        }
        if(playerSaveData.Consumable != null)
        {
            foreach (var consumable in playerSaveData.Consumable)
            {
                if (!runTimeDictionary.ContainsKey(consumable.Slot))
                    runTimeDictionary.Add(consumable.Slot, consumable as Item);
                else
                    Debug.Log($"Inventory already contains Item {runTimeDictionary[consumable.Slot].Name} on slot: {consumable.Slot}. Trying to also add item: {consumable.Name} to the same slot.");
            }
        }
        if(playerSaveData.Misc != null)
        {
            foreach (var misc in playerSaveData.Misc)
            {
                if (!runTimeDictionary.ContainsKey(misc.Slot))
                    runTimeDictionary.Add(misc.Slot, misc as Item);
                else
                    Debug.Log($"Inventory already contains Item {runTimeDictionary[misc.Slot].Name} on slot: {misc.Slot}. Trying to also add item: {misc.Name} to the same slot.");
            }
        }
        Debug.Log($"Loaded inventory with: {runTimeDictionary.Count} Slots taken");
        return runTimeDictionary;

    }

    public void SaveInventory(Dictionary<string, ItemSlot> slots) // save inherited data for wearable/holdable data as well 
    {
        NetInventory playerSaveData = new NetInventory();
        List<Wearable> wearables = new List<Wearable>();
        List<Holdable> holdables = new List<Holdable>();
        List<Consumable> consumables = new List<Consumable>();
        List<Misc> miscs = new List<Misc>();

        foreach(var itemSlot in slots)
        {
            if (itemSlot.Value.RuntimeItem.Item is Wearable)
            {
                Wearable wearable = ScriptableObject.CreateInstance(typeof(Wearable)) as Wearable;
                wearable.Initialize(itemSlot.Value.RuntimeItem.Item, itemSlot.Key);
                wearables.Add(wearable);
            }
            if (itemSlot.Value.RuntimeItem.Item is Holdable)
            {
                Holdable holdable = ScriptableObject.CreateInstance(typeof(Holdable)) as Holdable;
                holdable.Initialize(itemSlot.Value.RuntimeItem.Item, itemSlot.Key);
                holdables.Add(holdable);
            }
            if (itemSlot.Value.RuntimeItem.Item is Consumable)
            {
                Consumable consumable = ScriptableObject.CreateInstance(typeof(Consumable)) as Consumable;
                consumable.Initialize(itemSlot.Value.RuntimeItem.Item, itemSlot.Key);
                consumables.Add(consumable);
            }
            if (itemSlot.Value.RuntimeItem.Item is Misc)
            {
                Misc misc = ScriptableObject.CreateInstance(typeof(Misc)) as Misc;
                misc.Initialize(itemSlot.Value.RuntimeItem.Item, itemSlot.Key);
                miscs.Add(misc);
            }
        }

        playerSaveData.Wearables = wearables.ToArray();
        playerSaveData.Holdables = holdables.ToArray();
        playerSaveData.Consumable = consumables.ToArray();
        playerSaveData.Misc = miscs.ToArray();

        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        string json = JsonConvert.SerializeObject(playerSaveData, settings);
        File.WriteAllText(Application.persistentDataPath + "/GAPData.json", json);
        Debug.Log($"Saved inventory to: {Application.persistentDataPath}/GAPData.json");
    }

    public RuntimeItem LoadRuntimeItem(Item item, int playerID)
    {
        RuntimeItem runtimeItem = new RuntimeItem(item);
        StartCoroutine(LoadItem(runtimeItem, playerID));
        return runtimeItem;
    }

    public void LoadRuntimeItem(List<Item> items, Func<List<RuntimeItem>, int> callback)
    {
        List<RuntimeItem> runItems = new List<RuntimeItem>();
        foreach (var item in items)
        {
            runItems.Add(new RuntimeItem(item));
        }

        StartCoroutine(LoadItem(runItems, callback));
    }

    IEnumerator LoadItem(RuntimeItem rItem, int playerID)
    {
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> iconHandle = Addressables.LoadAssetAsync<Sprite>(rItem.Item.IconAddress);
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> prefabHandle = Addressables.LoadAssetAsync<GameObject>(rItem.Item.PrefabAddress);
        yield return iconHandle;
        yield return prefabHandle;

        while (!prefabHandle.IsDone || !iconHandle.IsDone)
            yield return new WaitForEndOfFrame();


        if (iconHandle.Result != null)
        {
            rItem.Icon = iconHandle.Result;
        }
        else
            Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");

        if (prefabHandle.Result != null)
        {
            rItem.Prefab = prefabHandle.Result;
        }
        else
            Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");


        _signalBus.Fire(new ItemLoadedSignal() { LoadedItem = rItem, PlayerID = playerID });
    }

    IEnumerator LoadItem(List<RuntimeItem> rItems, Func<List<RuntimeItem>, int> callback)
    {
        foreach (var rItem in rItems)
        {
            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> prefabHandle = Addressables.LoadAssetAsync<GameObject>(rItem.Item.PrefabAddress);
            yield return prefabHandle;

            while (!prefabHandle.IsDone)
                yield return new WaitForEndOfFrame();


            if (prefabHandle.Result != null)
            {
                rItem.Prefab = prefabHandle.Result;
            }
            else
                Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");

        }
        callback(rItems);
    }

    public IEnumerator LoadSingle(string adddress, Func<GameObject, int> callback)
    {
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> prefabHandle = Addressables.LoadAssetAsync<GameObject>(adddress);
        yield return prefabHandle;

        while (!prefabHandle.IsDone)
            yield return new WaitForEndOfFrame();


        GameObject thing2Load = null;
        if (prefabHandle.Result != null)
        {
            thing2Load = prefabHandle.Result;
        }
        else
            Debug.Log($"Failed to load {adddress}");

        callback(thing2Load);
    }

    public void LoadBuilding(BuildingData building)
    {
        GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>($"Buildings/{building.Name}"), building.Position, building.Rotation);
        instance.gameObject.SetActive(true);
    }

    public void UnloadBuilding(BuildingData building)
    {
    }

    public string GetInventoryJson()
    {
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/GAPData.json");
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        return jsonString;
    }
}

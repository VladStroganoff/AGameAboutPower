using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

public class WearableLoadedSignal
{
    public RuntimeItem RuntimeWear;
}

public class HoldableLoadedSignal
{
    public RuntimeItem RuntimeHoldable;
}

public class LoadController : MonoBehaviour, ILoadController
{
    string localSavePath = "";
    SignalBus _signalBus;
   
    [Inject]
    public void Inject(SignalBus bus)
    {
        _signalBus = bus;
    }


    public Dictionary<string, Item> LoadInventory()
    {
        string jsonString = File.ReadAllText(Application.persistentDataPath + "/GAPData.json");
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        JsonItemDictionary playerSaveData = JsonConvert.DeserializeObject(jsonString, settings) as JsonItemDictionary;

        Dictionary<string, Item> runTimeDictionary = new Dictionary<string, Item>();

        if(playerSaveData.Wearables != null)
        {
            foreach (var wearable in playerSaveData.Wearables)
            {
                if (!runTimeDictionary.ContainsKey(wearable.Slot))
                    runTimeDictionary.Add(wearable.Slot, wearable as Item);
                else
                    Debug.Log($"Inventory already contains Item {runTimeDictionary[wearable.Slot].Name} on slot: {wearable.Slot}. Trying to also add item: {wearable.Name} to the same slot.");
            }
        }
        if(playerSaveData.Holdables != null)
        {
            foreach (var holdable in playerSaveData.Holdables)
            {
                if (!runTimeDictionary.ContainsKey(holdable.Slot))
                    runTimeDictionary.Add(holdable.Slot, holdable as Item);
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

    public void SaveInventory(Dictionary<string, ItemSlot> slots)
    {
        JsonItemDictionary playerSaveData = new JsonItemDictionary();
        List<Wearable> wearables = new List<Wearable>();
        List<Holdable> holdables = new List<Holdable>();
        List<Consumable> consumables = new List<Consumable>();
        List<Misc> miscs = new List<Misc>();

        foreach(var itemSlot in slots)
        {
            if (itemSlot.Value.Item is Wearable)
            {
                Wearable wearable = ScriptableObject.CreateInstance(typeof(Wearable)) as Wearable;
                wearable.Initialize(itemSlot.Value.Item, itemSlot.Key);
                wearables.Add(wearable);
            }
            if (itemSlot.Value.Item is Holdable)
            {
                Holdable holdable = ScriptableObject.CreateInstance(typeof(Holdable)) as Holdable;
                holdable.Initialize(itemSlot.Value.Item, itemSlot.Key);
                holdables.Add(holdable);
            }
            if (itemSlot.Value.Item is Consumable)
            {
                Consumable consumable = ScriptableObject.CreateInstance(typeof(Consumable)) as Consumable;
                consumable.Initialize(itemSlot.Value.Item, itemSlot.Key);
                consumables.Add(consumable);
            }
            if (itemSlot.Value.Item is Misc)
            {
                Misc misc = ScriptableObject.CreateInstance(typeof(Misc)) as Misc;
                misc.Initialize(itemSlot.Value.Item, itemSlot.Key);
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

    public RuntimeItem LoadRuntimeItem(Item item)
    {
        RuntimeItem runtimeItem = new RuntimeItem(item);
        StartCoroutine(LoadItem(runtimeItem));
        return runtimeItem;
    }

    IEnumerator LoadItem(RuntimeItem rItem)
    {
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> iconHandle = Addressables.LoadAssetAsync<Sprite>(rItem.Item.IconAddress);
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> prefabHandle = Addressables.LoadAssetAsync<GameObject>(rItem.Item.PrefabAddress);
        yield return iconHandle;
        yield return prefabHandle;

        while (!prefabHandle.IsDone || !iconHandle.IsDone)
            yield return new WaitForEndOfFrame();


        if (iconHandle.Result != null)
        {
            Debug.Log($"Loaded Icon for: {rItem.Item.PrefabAddress}");
            rItem.Icon = iconHandle.Result;
        }
        else
            Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");

        if (prefabHandle.Result != null)
        {
            Debug.Log($"Loaded prefab for: {rItem.Item.PrefabAddress}");
            rItem.Prefab = prefabHandle.Result;
        }
        else
            Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");


        _signalBus.Fire(new WearableLoadedSignal() { RuntimeWear = rItem });
    }

    #region Crap
    public void LoadBuilding(BuildingData building)
    {
        GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>($"Buildings/{building.Name}"), building.Position, building.Rotation);
        instance.gameObject.SetActive(true);
    }

    public void UnloadBuilding(BuildingData building)
    {
    }

    public List<Item> LoadAllItems()
    {
        return default;
    }

    public void DestroyItem()
    {

    }


    public List<Item> ItemDataBase()
    {
        return default;
    }

    public List<Item> PlayerItems()
    {
        return default;
    }

    public GameObject LoadWearable(GameObject prefab) => Instantiate(prefab, Vector3.zero, Quaternion.identity);

    #endregion
}

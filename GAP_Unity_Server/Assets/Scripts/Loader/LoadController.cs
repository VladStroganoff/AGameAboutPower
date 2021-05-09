using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public delegate void WearLoaded(RuntimeItem runTime);


public class LoadController : MonoBehaviour
{
    public static LoadController instance;
    public WearLoaded loadedWear;

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


    public RuntimeItem LoadRuntimeItem(Item item)
    {
        RuntimeItem runtimeItem = new RuntimeItem(item);
        StartCoroutine(LoadItem(runtimeItem));
        return runtimeItem;
    }

    IEnumerator LoadItem(RuntimeItem rItem)
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

        loadedWear.Invoke(rItem);
    }

    public void LoadBuilding(BuildingData building)
    {
        GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>($"Buildings/{building.Name}"), building.Position, building.Rotation);
        instance.gameObject.SetActive(true);
    }

    public void UnloadBuilding(BuildingData building)
    {
    }

   
}

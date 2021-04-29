using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadController : MonoBehaviour, ILoadController
{
    string localSavePath = "";
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

    public void LoadInventory()
    {
    }

    public void SaveInventory(Dictionary<string, Item> Items)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        string json = JsonConvert.SerializeObject(Items, settings);
        File.WriteAllText(Application.persistentDataPath, json);
        Debug.Log($"Saved inventory to: {Application.persistentDataPath}");
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

        while (!iconHandle.IsDone && !iconHandle.IsDone)
            yield return new WaitForEndOfFrame();


        if (iconHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Loaded {rItem.Item.PrefabAddress}");
            rItem.Icon = iconHandle.Result;
        }
        else
            Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");

        if (prefabHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Loaded {rItem.Item.PrefabAddress}");
            rItem.Prefab = prefabHandle.Result;
        }
        else
            Debug.Log($"Failed to load {rItem.Item.PrefabAddress}");

    }
}

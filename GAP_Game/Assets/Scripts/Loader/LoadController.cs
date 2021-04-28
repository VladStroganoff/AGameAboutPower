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

    public IEnumerator LoadSprite(Item item, Image image)
    {
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> goHandle = Addressables.LoadAssetAsync<Sprite>(item.Icon);
        yield return goHandle;

        while (!goHandle.IsDone)
            yield return new WaitForEndOfFrame();

        if (goHandle.Status == AsyncOperationStatus.Succeeded)
        {
            image.sprite = goHandle.Result;
        }
    }

    public IEnumerator LoadPrefab(Item item)
    {
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> goHandle = Addressables.LoadAssetAsync<GameObject>(item.Prefab);
        yield return goHandle;

        while (!goHandle.IsDone)
            yield return new  WaitForEndOfFrame();

        if (goHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = goHandle.Result;
            yield return prefab;
        }
    }

    public void SaveInventory(Dictionary<string, Item> Items)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        string json = JsonConvert.SerializeObject(Items, settings);
        File.WriteAllText(Application.persistentDataPath, json);
        Debug.Log($"Saved inventory to: {Application.persistentDataPath}");
    }
}

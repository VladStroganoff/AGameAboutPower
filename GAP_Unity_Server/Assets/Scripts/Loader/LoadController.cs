using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class LoadController : MonoBehaviour
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

    public RuntimeItem LoadRuntimeItem(Item item, DressController dresser)
    {
        RuntimeItem runtimeItem = new RuntimeItem(item);
        StartCoroutine(LoadItem(runtimeItem, dresser));
        return runtimeItem;
    }

    public void LoadRuntimeItem(List<Item> items, LootView lootView)
    {
        List<RuntimeItem> runItems = new List<RuntimeItem>();
        foreach (var item in items)
        {
            runItems.Add(new RuntimeItem(item));
        }

        StartCoroutine(LoadItem(runItems, lootView));
    }

    IEnumerator LoadItem(RuntimeItem rItem, DressController dresser)
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

        dresser.AddWear(rItem);
    }

    IEnumerator LoadItem(List<RuntimeItem> rItems, LootView lootView)
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
        lootView.Populate(rItems);
    }

    public IEnumerator LoadSingle(string adddress, ItemReceiver recviever)
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

        recviever.RecieveItem(thing2Load);
    }

    public void LoadBuilding(BuildingData building)
    {
        GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>($"Buildings/{building.Name}"), building.Position, building.Rotation);
        instance.gameObject.SetActive(true);
    }


}

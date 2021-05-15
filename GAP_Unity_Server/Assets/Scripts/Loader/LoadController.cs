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

    public void LoadBuilding(BuildingData building)
    {
        GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>($"Buildings/{building.Name}"), building.Position, building.Rotation);
        instance.gameObject.SetActive(true);
    }
   

}

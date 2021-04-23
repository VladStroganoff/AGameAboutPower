using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadController : MonoBehaviour, ILoadController
{
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

}

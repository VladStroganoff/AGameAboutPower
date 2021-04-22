using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
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

}

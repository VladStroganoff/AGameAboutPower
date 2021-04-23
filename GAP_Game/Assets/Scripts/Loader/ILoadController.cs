using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadController
{
    void LoadBuilding(BuildingData building);
    void UnloadBuilding(BuildingData building);
    GameObject LoadWearable(GameObject address); // should be string
    List<Item> ItemDataBase();
    List<Item> PlayerItems();
    void DestroyItem();
}

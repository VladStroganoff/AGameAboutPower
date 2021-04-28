using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadController
{
    void LoadInventory();
    void SaveInventory(Dictionary<string, Item> Items);
    void LoadBuilding(BuildingData building);
    void UnloadBuilding(BuildingData building);
    //GameObject LoadPrefab(string address); 
    List<Item> ItemDataBase();
    List<Item> PlayerItems();
    void DestroyItem();
}

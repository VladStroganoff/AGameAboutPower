using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadController
{
    Dictionary<int, RuntimeItem> LoadInventory();
    void SaveInventory(Dictionary<string, Item> Items);
    void LoadBuilding(BuildingData building);
    void UnloadBuilding(BuildingData building);
    RuntimeItem LoadRuntimeItem(Item address); 
    List<Item> ItemDataBase();
    List<Item> PlayerItems();
    void DestroyItem();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadController
{
    Dictionary<string, Item> LoadInventory();
    void SaveInventory(Dictionary<string, ItemSlot> Items);
    void LoadBuilding(BuildingData building);
    void UnloadBuilding(BuildingData building);
    RuntimeItem LoadRuntimeItem(Item address); 
    List<Item> ItemDataBase();
    List<Item> PlayerItems();
    void DestroyItem();
}

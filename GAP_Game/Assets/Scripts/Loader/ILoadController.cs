using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadController
{
    string GetInventoryJson();
    Dictionary<string, Item> LoadInventory(string playerData);
    void SaveInventory(Dictionary<string, ItemSlot> Items);
    void LoadBuilding(BuildingData building);
    void UnloadBuilding(BuildingData building);
    RuntimeItem LoadRuntimeItem(Item address, int playerID); 
}

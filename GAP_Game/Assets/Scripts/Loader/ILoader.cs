using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader
{
    void LoadBuilding(BuildingData building);
    void UnloadBuilding(BuildingData building);
    List<Item> LoadAllItems();
    void DestroyItem();
}

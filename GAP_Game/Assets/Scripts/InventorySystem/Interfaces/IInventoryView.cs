using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryView
{
    void ShowPlayerInventiry(bool OnOff);
    void ShowLootInventory(bool OnOff);
    void CheckForLoot();
}

public interface IInventoryController
{
    void TakeItems(List<Item> Items);
    void DropItems(List<Item> Items);
}

public interface ILootController
{
    void ShowItems(List<Item> Items);
}

public interface IEquipController
{
    void EquipItem(Holdable wearableItem);
}
public interface IDressController
{
    void EquipItem(Wearable wearableItem);
}
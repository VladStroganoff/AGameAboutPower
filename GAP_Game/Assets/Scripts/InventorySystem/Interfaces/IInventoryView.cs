using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryView
{
    public Dictionary<string, ItemSlot> GetSlots();
    void ShowPlayerInventiry(bool OnOff);
    void ShowLootInventory(bool OnOff);
    void CheckForLoot();

    Dictionary<string, GameObject> GetWearSlots();
    void LoadInventiry(Dictionary<string, Item> items, int playerID, InventoryModel model);
}

public interface IInventoryController
{
    void SpawnPlayer(Dictionary<string, Item> items, PlayerManager player);

    public void ChangeInventory(int id, Item item);
    void TakeItems(List<Item> Items);
    void DropItems(List<Item> Items);
    void CheckGameState(GameStateChangedSignal signal);
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
    void InitializePlayer(Dictionary<string, Item> items, int playerID);
    void AddLoadedWear(RuntimeItem runItem);
    void SendSwapRequest(RuntimeItem runItem);
    void RecieveSwapWear(RuntimeItem runItem);
    void EquipItem(Wearable wearableItem);
}
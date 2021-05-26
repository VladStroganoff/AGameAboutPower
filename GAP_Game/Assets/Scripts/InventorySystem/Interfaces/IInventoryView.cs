using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryView
{
    public Dictionary<string, ItemSlot> GetSlots();// for testing
    Dictionary<string, GameObject> GetWearSlots();
    void LoadLoot(List<Item> items, int playrID, int lootID);
}

public interface IInventoryController
{
    void SpawnPlayer(Dictionary<string, Item> items, PlayerManager player);
    public void ChangeInventory(int id, Item item);
    void ShowLoot(NetLoot Items);
    void TakeItems(Dictionary<string, ItemSlot> inventory, List<ItemSlot> remainingLoot, int lootID);
    void DropItems(List<Item> Items);
    void CheckGameState(GameStateChangedSignal signal);
}



public interface IEquipController
{
    void EquipItem(Holdable wearableItem);
}
public interface IDressController
{
    void InitializePlayer(Dictionary<string, Item> items, int playerID);
    void AddLoadedWear(RuntimeItem runItem);
    void SendSwapRequest(RuntimeItem newWear, RuntimeItem oldWear);
    void RecieveSwapWear(RuntimeItem runItem);
    void EquipItem(Wearable wearableItem);
}
using System.Collections.Generic;



public interface IInventoryController
{
    void SpawnOtherPlayer(Dictionary<string, Item> items);
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
    void AddWear(RuntimeItem runItem);

    void SwapWear(RuntimeItem runItem);

    void EquipItem(Wearable wearableItem);
}
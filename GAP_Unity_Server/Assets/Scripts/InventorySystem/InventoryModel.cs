using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    public Dictionary<string, RuntimeItem> Inventory = new Dictionary<string, RuntimeItem>();
    public JsonItemDictionary JsonInventory;
    JsonSerializerSettings _settings;
    Player _player;
    public void InitializeInventory(string playerData, Player player)
    {
        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        JsonInventory = JsonConvert.DeserializeObject(playerData, _settings) as JsonItemDictionary;
        _player = player;
        player.playerData = playerData;
        GetComponent<DressController>().InitializeWear(JsonInventory);
    }
    public void AddItemToPlayer(RuntimeItem runItem)
    {
        if (!Inventory.ContainsKey(runItem.Item.Slot))
            Inventory.Add(runItem.Item.Slot, runItem);
        else
            Inventory[runItem.Item.Slot] = runItem;

        if(runItem.Item is Wearable)
        {
            for(int i =0; i < JsonInventory.Wearables.Length; i++)
            {
                if (JsonInventory.Wearables[i].Slot == runItem.Item.Slot)
                    JsonInventory.Wearables[i] = runItem.Item as Wearable;
            }
        }
        if (runItem.Item is Holdable)
        {
            for (int i = 0; i < JsonInventory.Wearables.Length; i++)
            {
                if (JsonInventory.Holdables[i].Slot == runItem.Item.Slot)
                    JsonInventory.Holdables[i] = runItem.Item as Holdable;
            }
        }
        if (runItem.Item is Consumable)
        {
            for (int i = 0; i < JsonInventory.Wearables.Length; i++)
            {
                if (JsonInventory.Consumable[i].Slot == runItem.Item.Slot)
                    JsonInventory.Consumable[i] = runItem.Item as Consumable;
            }
        }
        if (runItem.Item is Misc)
        {
            for (int i = 0; i < JsonInventory.Wearables.Length; i++)
            {
                if (JsonInventory.Misc[i].Slot == runItem.Item.Slot)
                    JsonInventory.Misc[i] = runItem.Item as Misc;
            }
        }

        _player.playerData = JsonConvert.SerializeObject(JsonInventory, _settings);
    }

}

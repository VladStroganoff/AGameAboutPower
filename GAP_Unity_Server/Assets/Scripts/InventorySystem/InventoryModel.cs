using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    public Dictionary<string, RuntimeItem> Inventory = new Dictionary<string, RuntimeItem>();
    public NetInventory NetInventory;
    JsonSerializerSettings _settings;
    PlayerManager _player;
    public void InitializeInventory(string playerData, PlayerManager player)
    {
        _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        NetInventory = JsonConvert.DeserializeObject(playerData, _settings) as NetInventory;
        _player = player;
        player.playerData = playerData;
        GetComponent<DressController>().InitializeWear(NetInventory);
    }
    public void AddItemToPlayer(RuntimeItem runItem)
    {
        if (!Inventory.ContainsKey(runItem.Item.Slot))
            Inventory.Add(runItem.Item.Slot, runItem);
        else
            Inventory[runItem.Item.Slot] = runItem;

        if(runItem.Item is Wearable)
        {
            for(int i =0; i < NetInventory.Wearables.Length; i++)
            {
                if (NetInventory.Wearables[i].Slot == runItem.Item.Slot)
                    NetInventory.Wearables[i] = runItem.Item as Wearable;
            }
        }
        if (runItem.Item is Holdable)
        {
            for (int i = 0; i < NetInventory.Wearables.Length; i++)
            {
                if (NetInventory.Holdables[i].Slot == runItem.Item.Slot)
                    NetInventory.Holdables[i] = runItem.Item as Holdable;
            }
        }
        if (runItem.Item is Consumable)
        {
            for (int i = 0; i < NetInventory.Wearables.Length; i++)
            {
                if (NetInventory.Consumable[i].Slot == runItem.Item.Slot)
                    NetInventory.Consumable[i] = runItem.Item as Consumable;
            }
        }
        if (runItem.Item is Misc)
        {
            for (int i = 0; i < NetInventory.Wearables.Length; i++)
            {
                if (NetInventory.Misc[i].Slot == runItem.Item.Slot)
                    NetInventory.Misc[i] = runItem.Item as Misc;
            }
        }

        _player.playerData = JsonConvert.SerializeObject(NetInventory, _settings);
    }

}

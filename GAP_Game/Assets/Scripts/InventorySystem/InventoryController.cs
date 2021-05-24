using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryController : MonoBehaviour, IInventoryController
{
    IInventoryView _inventoryView;
    ILoadController _loadControl;
    public bool testSave;

    Dictionary<int, InventoryModel> _playerInventories = new Dictionary<int, InventoryModel>();

    [Inject]
    public void Inject(IInventoryView inventoryView, ILoadController loadControl)
    {
        _inventoryView = inventoryView;
        _loadControl = loadControl;
    }

    public void ShowLoot(NetLoot netItems)
    {
        List<Item> newItmes = netItems.GetItems();
        _inventoryView.LoadLoot(newItmes, netItems.ownerID, netItems.lootID);
    }

    public void TakeItems(InventoryModel inventory, List<ItemSlot> remainingLoot, int lootID)
    {
        List<Item> items = new List<Item>();
        foreach(var loot in remainingLoot)
        {
            if(loot != null)
            {
                items.Add(loot.RuntimeItem.Item);
            }
        }
        NetLoot netLoot = new NetLoot(items);
        netLoot.lootID = lootID;
        NetInventory netInventory = inventory.MakeNetCopy();
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        string jsonInventory = JsonConvert.SerializeObject(netInventory, settings);
        string Jsonloot = JsonConvert.SerializeObject(netLoot, settings);
        ClientSend.SendJsonPackage(jsonInventory);
        ClientSend.SendJsonPackage(Jsonloot);
    }

    public void DropItems(List<Item> Items)
    {
    }

    public void CheckGameState(GameStateChangedSignal signal) // maybe I wont need this
    {
        if (signal.state == GameState.InLobby)
            return;
    }

    void Start()
    {
        if(testSave)
            TestSave();
    }
    void TestSave()
    {
        Dictionary<string, ItemSlot> slots = _inventoryView.GetSlots(); // these slots are populated on the gui just for testing
        _loadControl.SaveInventory(slots);
    }

    public void SpawnPlayer(Dictionary<string, Item> items, PlayerManager player)
    {
        if(!_playerInventories.ContainsKey(player.id))
        {
            var inventory = player.GetComponent<InventoryModel>();
            inventory.ID = player.id;
            _playerInventories.Add(player.id, inventory);
        }

        player.GetComponent<DressController>().InitializePlayer(items, player.id);

    }
    public void ChangeInventory(int id, Item item)
    {
        if(_playerInventories.ContainsKey(id))
        {
            _playerInventories[id].LoadItem(item);
        }
    }

   
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryController : MonoBehaviour, IInventoryController
{
    IInventoryView _inventoryView;
    ILoadController _loadControl;
    public bool testSave;

    [Inject]
    public void Inject(IInventoryView inventoryView, ILoadController loadControl)
    {
        _inventoryView = inventoryView;
        _loadControl = loadControl;
    }

    public void TakeItems(List<Item> Items)
    {
    }
    public void DropItems(List<Item> Items)
    {
    }

    public void CheckGameState(GameStateChangedSignal signal)
    {
        if (signal.state == GameState.InLobby)
            return;

        if (signal.state == GameState.InGame)
        {
            //Dictionary<string, Item> items = _loadControl.LoadInventory();
            //_inventoryView.LoadInventiry(items);
        }
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
        if (player.id == GameClient.instance.myId)
        {
            _inventoryView.LoadInventiry(items, player.id);
        }
        else
        {
            player.GetComponent<DressController>().InitializeOtherPlayer(items, player.id);
        }

    }

    public void SpawnOtherPlayer(Dictionary<string, Item> items)
    {
        throw new System.NotImplementedException();
    }
}


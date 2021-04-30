using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InventoryController : MonoBehaviour, IInventoryController
{
    IInventoryView _inventoryView;
    ILoadController _loadControl;


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




     void Start()
    {
        //Dictionary<int, RuntimeItem> items =  _loadControl.LoadInventory();
        //_inventoryView.PreloadToInventory(Dictionary<>)
        Dictionary<string, ItemSlot> slots = _inventoryView.GetSlots(); // these slots are populated on the gui just for testing
        Dictionary<string, Item> SaveItems = new Dictionary<string, Item>();

        foreach (var pair in slots)
            SaveItems.Add(pair.Key, pair.Value.Item);

        _loadControl.SaveInventory(SaveItems);
    }
}

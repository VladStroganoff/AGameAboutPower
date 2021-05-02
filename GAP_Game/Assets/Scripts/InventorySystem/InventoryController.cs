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
        //TestSave();
        TestLoad();
    }
    void TestLoad()
    {
        Dictionary<string, Item> items = _loadControl.LoadInventory();
        _inventoryView.PreloadToInventory(items);
    }
    void TestSave()
    {
        Dictionary<string, ItemSlot> slots = _inventoryView.GetSlots(); // these slots are populated on the gui just for testing
        _loadControl.SaveInventory(slots);
    }

}


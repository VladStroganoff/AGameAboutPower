using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour, IInventoryView
{
    public RectTransform InventoryPanel;
    Vector2 _inventoryOrigin;
    bool _toggleInventory = false;

    public RectTransform LootPanel;
    Vector2 _lootOrigin;
    bool _toggleLoot = false;

    public RectTransform ItemsRect;
    public float ItemStandardSize;
    public Dictionary<string, ItemSlot> ItemSlots = new Dictionary<string, ItemSlot>();

    void OnValidate()
    {
        ItemSlot[] slots = gameObject.GetComponentsInChildren<ItemSlot>();
        foreach(var slot in slots)
        {
            ItemSlots.Add(slot.gameObject.name, slot);
        }

    }

    void Start()
    {
        _inventoryOrigin = InventoryPanel.anchoredPosition;
        _lootOrigin = LootPanel.anchoredPosition;
    }


    public void CheckForLoot()
    {
    }

    public void ShowLootInventory(bool OnOff)
    {
    }

    public void ShowPlayerInventiry(bool OnOff)
    {
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I)) 
        {
            ToggleInventory();
        }

    }

    public Dictionary<string, ItemSlot> GetSlots() => ItemSlots;
    public void PreloadToInventory(Dictionary<string, Item> items)
    {

    }

    public void AddItemToInventory(RuntimeItem newItem)
    {

    }

    void ToggleInventory()
    {
        if (!_toggleInventory)
        {
            InventoryPanel.anchoredPosition = new Vector2(InventoryPanel.anchoredPosition.x, 0);
            _toggleInventory = !_toggleInventory;
        }
        else
        {
            InventoryPanel.anchoredPosition = _inventoryOrigin;
            _toggleInventory = !_toggleInventory;
        }
    }
    
}

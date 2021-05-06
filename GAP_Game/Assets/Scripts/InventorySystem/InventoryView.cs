using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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
    ILoadController _loadControl;


    [Inject]
    public void Inject(ILoadController loadControl, SignalBus signalBus)
    {
        //Debug.Log("Inventory veiw controller gets injected");
        _loadControl = loadControl;
        signalBus.Subscribe<ItemLoadedSignal>(ItemLoaded);
    }

    void Start()
    {
        CollectItems();
        _inventoryOrigin = InventoryPanel.anchoredPosition;
        _lootOrigin = LootPanel.anchoredPosition;
    }
    void CollectItems()
    {
        ItemSlots.Clear();
        ItemSlot[] slots = gameObject.GetComponentsInChildren<ItemSlot>();
        foreach (var slot in slots)
        {
            ItemSlots.Add(slot.gameObject.name.ToString(), slot);
        }
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

    public Dictionary<string, ItemSlot> GetSlots()
    {
        foreach (var item in ItemSlots)
        {
            if (item.Value.RuntimeItem.Item != null)
                item.Value.RuntimeItem.Item.Slot = item.Key;
        }
        return ItemSlots;
    }
    public void LoadInventiry(Dictionary<string, Item> items)
    {
        foreach(var item in items)
        {
            RuntimeItem runtimeItem = _loadControl.LoadRuntimeItem(item.Value);
            ItemSlots[item.Key].RuntimeItem = runtimeItem;
        }
    }

    public void ItemLoaded(ItemLoadedSignal runItem)
    {
        if (ItemSlots.ContainsKey(runItem.LoadedItem.Item.Slot))
        {
            ItemSlots[runItem.LoadedItem.Item.Slot].RuntimeItem = runItem.LoadedItem;
            ItemSlots[runItem.LoadedItem.Item.Slot].Populate();
        }

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

    public Dictionary<string, GameObject> GetWearSlots()
    {
        Dictionary<string, GameObject> wearSlots = new Dictionary<string, GameObject>();

        foreach(var slot in ItemSlots)
        {
            if (slot.Value.inUse == true)
                wearSlots.Add(slot.Key, null);

        }


        return wearSlots;
    }
}

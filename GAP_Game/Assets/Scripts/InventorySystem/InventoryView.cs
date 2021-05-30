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
    bool _updateLoot = false;

    public RectTransform ItemsRect;
    public float StandardPadding;
    public Dictionary<string, ItemSlot> ItemSlots = new Dictionary<string, ItemSlot>();
    public List<ItemSlot> LootSlots = new List<ItemSlot>();
    int _lootID = 0;
    SignalBus _signalBus;

    ILoadController _loadControl;
    IInventoryController _inventoryControl;


    [Inject]
    public void Inject(ILoadController loadControl, SignalBus signalBus, IInventoryController inventoryControl)
    {
        _loadControl = loadControl;
        _inventoryControl = inventoryControl;
        signalBus.Subscribe<ItemLoadedSignal>(ItemLoaded);
        _signalBus = signalBus;
        CollectItems();
    }

    void CollectItems()
    {
        _inventoryOrigin = InventoryPanel.anchoredPosition;
        _lootOrigin = LootPanel.anchoredPosition;
        ItemSlots.Clear();
        LootSlots.Clear();
        ItemSlot[] slots = gameObject.GetComponentsInChildren<ItemSlot>();
        foreach (var slot in slots)
        {
            if(slot is LootSlot)
            {
                LootSlots.Add(slot);
            }
            else
            {
                ItemSlots.Add(slot.gameObject.name.ToString(), slot);
                
            }
        }
    }
   
    public void LoadLoot(List<Item> items, int ownerID, int lootID)
    {
        int lootI = 0;
        foreach (var item in items)
        {
            item.Slot = lootI.ToString();
            RuntimeItem runtimeItem = _loadControl.LoadRuntimeItem(item, ownerID);
            LootSlots[lootI].RuntimeItem = runtimeItem;
            lootI++;
        }
        _lootID = lootID;
        ToggleInventory(true);
    }

    public void TakeAll() // from button
    {
        foreach(var lootSlot in LootSlots)
        {
            if(lootSlot._currentView != null)
            {
                foreach(var invSlot in ItemSlots)
                {
                    if (invSlot.Value is WearableSlot)
                        continue;
                    if (invSlot.Value is HoldableSlot)
                        continue;

                    if (invSlot.Value._currentView == null)
                    {
                        invSlot.Value.OnItemDropped(lootSlot._currentView, true);
                        break;
                    }
                }
            }
        }
    }

    public void ItemLoaded(ItemLoadedSignal runItem)
    {
        if (runItem.PlayerID != GameClient.instance.myId)
            return;

        if (ItemSlots.ContainsKey(runItem.LoadedItem.Item.Slot))
        {
            ItemSlots[runItem.LoadedItem.Item.Slot].RuntimeItem = runItem.LoadedItem;
            ItemSlots[runItem.LoadedItem.Item.Slot].Populate(StandardPadding);
        }
        else
        {
            int slot = 0;
            int.TryParse(runItem.LoadedItem.Item.Slot, out slot);
            LootSlots[slot].RuntimeItem = runItem.LoadedItem;
            LootSlots[slot].Populate(StandardPadding);
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

    void UpdateInventory()
    {
        _inventoryControl.TakeItems(ItemSlots, LootSlots, _lootID);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            ToggleInventory(false);
        }
    }
    void ToggleInventory(bool withLoot)
    {
        if (!_toggleInventory)
        {
            InventoryPanel.anchoredPosition = new Vector2(InventoryPanel.anchoredPosition.x, 0);
            _toggleInventory = !_toggleInventory;
            _signalBus.Fire(new CameraStateSignal() { state = CameraState.Menu });
            if(withLoot)
            {
                LootPanel.anchoredPosition = new Vector2(LootPanel.anchoredPosition.x, 0);
                _updateLoot = true;
            }
        }
        else
        {
            _signalBus.Fire(new CameraStateSignal() { state = CameraState.TPS });
            InventoryPanel.anchoredPosition = _inventoryOrigin;
            _toggleInventory = !_toggleInventory;
            LootPanel.anchoredPosition = _lootOrigin;
            if (_updateLoot)
            {
                _updateLoot = false;
                UpdateInventory();
            }
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
}

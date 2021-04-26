using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour, IInventoryView
{
    public RectTransform InventoryPanel;
    Vector2 _inventoryOrigin;

    public RectTransform LootPanel;
    Vector2 _lootOrigin;

    public RectTransform HeadSlot;
    public RectTransform TorsoSlot;
    public RectTransform RightArmSlot;
    public RectTransform LeftArmSlot;
    public RectTransform LegsSlot;
    public RectTransform LeftRifleSlot;
    public RectTransform RightRifleSlot;
    public RectTransform LeftPistolSlot;
    public RectTransform RightPistolSlot;

    public RectTransform BackPack;
    public List<RectTransform> BackPackSlots;

    public RectTransform Loot;
    public List<RectTransform> LootSlots;

    private void OnValidate()
    {
        LootSlots.Clear();
        BackPackSlots.Clear();


        if (BackPack == null)
            return;
        if (BackPack.GetComponent<RectTransform>() == null)
            return;

        foreach (Transform child in BackPack.transform)
        {
            BackPackSlots.Add(child.gameObject.GetComponent<RectTransform>());
        }

        if (Loot == null)
            return;
        if (Loot.GetComponent<RectTransform>() == null)
            return;

        foreach (Transform child in Loot.transform)
        {
            LootSlots.Add(child.gameObject.GetComponent<RectTransform>());
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
    
}

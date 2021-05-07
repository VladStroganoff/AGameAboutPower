using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    protected DropArea _dropArea;
    public RuntimeItem RuntimeItem;
    protected ItemView _currentView;
    public bool inUse;

    public virtual void Populate(float ItemStandardSize) // maybe this should not be hard coded
    {
        if (RuntimeItem.Item == null)
            return;


        GameObject item = new GameObject("Item-" + RuntimeItem.Item.Name);
        item.transform.SetParent(transform);
        RectTransform rect = item.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(ItemStandardSize, ItemStandardSize);
        Image image = item.AddComponent<Image>();
        image.sprite = RuntimeItem.Icon;
        rect.anchoredPosition = Vector2.zero;
        _currentView = item.AddComponent<ItemView>();
        _currentView.RuntimeItem = RuntimeItem;
    }

    protected virtual void Awake()
    {
        _dropArea = GetComponent <DropArea>() ?? gameObject.AddComponent<DropArea>();
        _dropArea.OnDropHandler += OnItemDropped;
    }
    public virtual void OnItemDropped(ItemView draggable)
    {
        RuntimeItem = draggable.RuntimeItem;
        draggable.transform.position = transform.position;
        draggable.transform.SetParent(transform);
        draggable.RuntimeItem.Item.Slot = gameObject.name; 
    }
}

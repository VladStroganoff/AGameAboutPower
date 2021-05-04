using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    protected DropArea _dropArea;
    public RuntimeItem RuntimeItem;
    public bool inUse;

    public virtual void Populate() // maybe this should not be hard coded
    {
        if (RuntimeItem.Item == null)
            return;

        GameObject item = new GameObject("Item-" + RuntimeItem.Item.Name);
        item.transform.SetParent(transform);
        RectTransform rect = item.AddComponent<RectTransform>();
        Image image = item.AddComponent<Image>();
        image.sprite = RuntimeItem.Icon;
        rect.anchoredPosition = Vector2.zero;
        ItemView view = item.AddComponent<ItemView>();
        view.RuntimeItem = RuntimeItem;
    }

    protected virtual void Awake()
    {
        _dropArea = GetComponent <DropArea>() ?? gameObject.AddComponent<DropArea>();
        _dropArea.OnDropHandler += OnItemDropped;
    }
    private void OnItemDropped(ItemView draggable)
    {
        RuntimeItem = draggable.RuntimeItem;
        draggable.transform.position = transform.position;
        draggable.transform.SetParent(transform);
    }
}

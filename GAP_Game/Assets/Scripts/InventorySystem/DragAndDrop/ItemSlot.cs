using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ItemSlot : MonoBehaviour
{
    protected DropArea _dropArea;
    public RuntimeItem RuntimeItem;
    protected ItemView _currentView;
    public bool inUse;
    RectTransform _RectTranfrom;
    float _padding;
    [Inject]
    public void Inject(SignalBus bus)
    {
        bus.Subscribe<PlayerSpawned>(PopulateSlot);
        RuntimeItem = null;
    }

    public virtual void Populate(float padding) // maybe this should not be hard coded
    {
        if (RuntimeItem.Item == null)
            return;

        _padding = padding;
        GameObject item = new GameObject("Item-" + RuntimeItem.Item.Name);

        var itemRect = item.AddComponent<RectTransform>();
        itemRect.anchoredPosition = _RectTranfrom.position;

        Image image = item.AddComponent<Image>();
        image.preserveAspect = true;
        image.sprite = RuntimeItem.Icon;

        itemRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.transform.SetParent(transform);
        itemRect.sizeDelta = new Vector2(_RectTranfrom.rect.size.x - _padding, _RectTranfrom.rect.size.y - _padding);
        itemRect.localScale = new Vector3(1, 1, 1);

        _currentView = item.AddComponent<ItemView>();
        _currentView.RuntimeItem = RuntimeItem;
        

    }

    public virtual void PopulateSlot(PlayerSpawned playerSpanwed)
    {
        playerSpanwed.player.GetComponent<InventoryModel>().AddSlot(gameObject.name);
    }

    protected virtual void Awake()
    {
        _dropArea = GetComponent <DropArea>() ?? gameObject.AddComponent<DropArea>();
        _dropArea.OnDropHandler += OnItemDropped;
        _RectTranfrom = GetComponent<RectTransform>();
    }
    public virtual void OnItemDropped(ItemView draggable)
    {
        draggable.currentParent.GetComponent<ItemSlot>().RuntimeItem = null;

        draggable.transform.position = transform.position;

        draggable.transform.SetParent(transform);

        RuntimeItem = draggable.RuntimeItem;
        draggable.RuntimeItem.Item.Slot = gameObject.name; 
    }
}

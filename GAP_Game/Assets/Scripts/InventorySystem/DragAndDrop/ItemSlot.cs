using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    protected DropArea _dropArea;
    public Item Item;
    public string name;
    protected virtual void Awake()
    {
        _dropArea = GetComponent <DropArea>() ?? gameObject.AddComponent<DropArea>();
        _dropArea.OnDropHandler += OnItemDropped;
    }

    private void OnValidate()
    {
        name = gameObject.name;
    }

    private void OnItemDropped(ItemView draggable)
    {
        Item = draggable.Item;
        draggable.transform.position = transform.position;
    }
}

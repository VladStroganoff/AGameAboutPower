using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    protected DropArea _dropArea;

    protected virtual void Awake()
    {
        _dropArea = GetComponent <DropArea>() ?? gameObject.AddComponent<DropArea>();
        _dropArea.OnDropHandler += OnItemDropped;
    }

    private void OnItemDropped(DragableItem draggable)
    {
        draggable.transform.position = transform.position;
    }
}

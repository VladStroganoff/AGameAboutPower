using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public List<DropCondition> DropCondition = new List<DropCondition>();
    public event Action<DragableItem> OnDropHandler;

    public bool Accepts(DragableItem draggable)
    {
        return DropCondition.TrueForAll(cond => cond.Check(draggable));
    }

    public void Drop(DragableItem draggable)
    {
        OnDropHandler?.Invoke(draggable);
    }
}

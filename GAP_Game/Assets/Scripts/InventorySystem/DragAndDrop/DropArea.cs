using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropArea : MonoBehaviour
{
    public List<DropCondition> DropCondition = new List<DropCondition>();
    public event Action<ItemView> OnDropHandler;

    public bool Accepts(ItemView draggable)
    {
        return DropCondition.TrueForAll(cond => cond.Check(draggable));
    }

    public void Drop(ItemView draggable)
    {
        OnDropHandler?.Invoke(draggable);
    }
}

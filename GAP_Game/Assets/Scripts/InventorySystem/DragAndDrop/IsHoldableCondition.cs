using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsHoldableCondition : DropCondition
{
    public override bool Check(DragableItem draggable)
    {
        if (draggable.GetComponent<DragableItem>().Item is Holdable)
            return true;
        else
            return false;
    }
}

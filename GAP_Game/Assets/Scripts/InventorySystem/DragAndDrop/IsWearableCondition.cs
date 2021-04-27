using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWearableCondition : DropCondition
{
    public override bool Check(DragableItem draggable)
    {
        if (draggable.GetComponent<DragableItem>().Item is Wearable)
            return true;
        else
            return false;
    }
}

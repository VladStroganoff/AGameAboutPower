using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsHoldableCondition : DropCondition
{
    public override bool Check(ItemView draggable)
    {
        if (draggable.GetComponent<ItemView>().RuntimeItem is Holdable)
            return true;
        else
            return false;
    }
}

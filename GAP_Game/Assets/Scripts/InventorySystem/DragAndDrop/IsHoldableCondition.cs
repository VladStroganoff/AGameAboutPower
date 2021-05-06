using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsHoldableCondition : DropCondition
{
    HoldableType _type;
    public IsHoldableCondition(HoldableType type)
    {
        _type = type;
    }

    public override bool Check(ItemView draggable)
    {
        if (draggable.GetComponent<ItemView>().RuntimeItem.Item is Holdable)
        {
            Holdable holdable = draggable.GetComponent<ItemView>().RuntimeItem.Item as Holdable;

            
            if (holdable.Type == _type)
                return true;
            else
                return false;
        }
        else
            return false;
    }
}

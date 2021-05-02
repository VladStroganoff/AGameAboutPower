using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWearableCondition : DropCondition
{
    public override bool Check(ItemView draggable)
    {
        if (draggable.GetComponent<ItemView>().Item is Wearable)
            return true;
        else
            return false;
    }
}
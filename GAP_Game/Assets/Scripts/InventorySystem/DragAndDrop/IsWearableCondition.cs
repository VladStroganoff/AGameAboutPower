using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWearableCondition : DropCondition
{
    BodyPart _type;


    public IsWearableCondition(BodyPart type)
    {
        _type = type;
    }

    public override bool Check(ItemView draggable)
    {
        if (draggable.GetComponent<ItemView>().RuntimeItem.Item is Wearable)
        {
            Wearable wear = draggable.GetComponent<ItemView>().RuntimeItem.Item as Wearable;

            if (wear.Type == _type)
                return true;
            else
                return false;
        }
        else
            return false;
    }
}

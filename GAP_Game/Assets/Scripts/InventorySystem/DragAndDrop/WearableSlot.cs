using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearableSlot : ItemSlot
{
    protected override void Awake()
    {
        base.Awake();
        _dropArea.DropCondition.Add(new IsWearableCondition());
    }
}

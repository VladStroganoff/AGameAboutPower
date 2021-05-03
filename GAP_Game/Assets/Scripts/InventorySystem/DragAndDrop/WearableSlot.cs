using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class WearableSlot : ItemSlot
{
    DressController _dressControl;

    public void Start()
    {
        _dressControl = GameObject.FindObjectOfType<DressController>();
    }

    protected override void Awake()
    {
        base.Awake();
        _dropArea.DropCondition.Add(new IsWearableCondition());
    }

    public override void Populate()
    {
        base.Populate();
        _dressControl.AddWear(RuntimeItem);
    }

}

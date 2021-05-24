using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSlot : ItemSlot
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Populate(float ItemStandardSize)
    {
        base.Populate(ItemStandardSize);
    }

    public override void PopulateSlot(PlayerSpawned playerSpanwed)
    {
        base.PopulateSlot(playerSpanwed);
    }

    public override void OnItemDropped(ItemView draggable)
    {
        base.OnItemDropped(draggable);
    }
}

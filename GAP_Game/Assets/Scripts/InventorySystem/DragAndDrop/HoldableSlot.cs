using UnityEngine;
public class HoldableSlot : ItemSlot
{
    public HoldableType Type;
    protected override void Awake()
    {
        base.Awake();
        //Debug.Log($"{gameObject.name} set to {Type}");
        _dropArea.DropCondition.Add(new IsHoldableCondition(Type));
    }

    public override void Populate(float ItemStandardSize)
    {
        base.Populate(ItemStandardSize);
    }

    public override void OnItemDropped(ItemView draggable)
    {
        base.OnItemDropped(draggable);
    }
}

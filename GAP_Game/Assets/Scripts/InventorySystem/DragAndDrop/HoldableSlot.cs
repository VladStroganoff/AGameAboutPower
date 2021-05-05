using UnityEngine;
public class HoldableSlot : ItemSlot
{
    public HoldableType Type;
    protected override void Awake()
    {
        base.Awake();
        Debug.Log($"{gameObject.name} set to {Type}");
        _dropArea.DropCondition.Add(new IsHoldableCondition(Type));
    }
}

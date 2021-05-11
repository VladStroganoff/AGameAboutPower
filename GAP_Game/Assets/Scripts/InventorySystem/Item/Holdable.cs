using UnityEngine;
[System.Serializable]
public enum HoldableType { Rifle, Pistol, Melee}

[System.Serializable]
[CreateAssetMenu(fileName = "New Holdable Item", menuName = "InventoryItem/Holdable")]
public class Holdable : Item
{
    public int Damage;
    public HoldableType Type;

    public override void Initialize(Item item, string slot)
    {
        base.Initialize(item, slot);
        Holdable hold = item as Holdable;
        Damage = hold.Damage;
        Type = hold.Type;
    }


    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}

public class NetHoldable : NetItem
{
    public int Damage;
    public HoldableType Type;

    public override void Inlitialize(Item item)
    {
        base.Inlitialize(item);
        Holdable hold = item as Holdable;
        Damage = hold.Damage;
        Type = hold.Type;
    }
}
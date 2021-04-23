using UnityEngine;

[CreateAssetMenu(fileName = "New Holdable Item", menuName = "InventoryItem/Holdable")]
public class Holdable : Item
{
    public int Damage;

    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
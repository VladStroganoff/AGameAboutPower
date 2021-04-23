using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wearable Item", menuName = "InventoryItem/Wearable")] // This cannot be assigned in inspector
public class Wearable : Item
{
    int Space;

    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
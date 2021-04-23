using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "InventoryItem/Consumable")]
public class Consumable : Item
{
    int weight;
    int Space;

    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
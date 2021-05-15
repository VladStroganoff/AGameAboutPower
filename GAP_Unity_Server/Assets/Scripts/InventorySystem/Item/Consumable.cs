using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
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


public class NetConsumable : NetItem
{ }

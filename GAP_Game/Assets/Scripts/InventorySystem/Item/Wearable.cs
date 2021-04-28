using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "New Wearable Item", menuName = "InventoryItem/Wearable")] // This cannot be assigned in inspector
public class Wearable : Item
{
    int Space;
    private void OnValidate()
    {

    }
    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryModel : MonoBehaviour
{
    public Dictionary<string, RuntimeItem> Inventory = new Dictionary<string, RuntimeItem>();
    public List<RuntimeItem> InventoryDisplay = new List<RuntimeItem>();

    void UpdateDisplay()
    {
        InventoryDisplay.Clear();
        InventoryDisplay.AddRange(Inventory.Values);
    }
}

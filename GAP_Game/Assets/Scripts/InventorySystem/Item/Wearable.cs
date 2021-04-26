using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wearable Item", menuName = "InventoryItem/Wearable")] // This cannot be assigned in inspector
public class Wearable : Item
{
    int Space;
    public List<string> _boneNames { get; private set; }
    private void OnValidate()
    {
        _boneNames.Clear();
        if (Prefab == null)
            return;
        if (Prefab.GetComponent<SkinnedMeshRenderer>() == null)
            return;

        var rend = Prefab.GetComponent<SkinnedMeshRenderer>();
        foreach (var transform in rend.bones)
        {
            _boneNames.Add(transform.name);
        }

    }
    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
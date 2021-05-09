using UnityEngine;
[System.Serializable]
public enum BodyPart { Head, LeftArm, RightArm, Torso, Legs, Jacket, Pants}

[System.Serializable]
[CreateAssetMenu(fileName = "New Wearable Item", menuName = "InventoryItem/Wearable")] // This cannot be assigned in inspector
public class Wearable : Item
{
    public BodyPart Type;
    int Space;
    string[] _boneNames;

    public override void Initialize(Item item, string slot)
    {
        base.Initialize(item, slot);
        Wearable wearData = item as Wearable;
        Type = wearData.Type;
        Space = wearData.Space;
        _boneNames = wearData._boneNames;
    }

    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
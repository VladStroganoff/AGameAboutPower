using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string Name;
    public int ID;
    public int Health;
    public GameObject Prefab; // should just be string
    public Texture2D Icon;
    public abstract void ActivateItem();
    public abstract void Deactivate();
}

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
[CreateAssetMenu(fileName = "New Misc Item", menuName = "InventoryItem/Misc")]
public class Misc : Item
{
    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Limb, Bag, Clothing, Weapon, Food, Ammo }
[CreateAssetMenu(fileName = "New Item", menuName ="Inventiry/Items")]
public class Item : ScriptableObject
{

    public string Name;
    public int ID;
    public ItemType Type;
    public GameObject Prefab;
    public Texture2D Icon;
    public int Health;
    public int Damage;
    public int CarryCapacity;
}

public class Food : Item
{
    int weight;
    int healthGen;
}

public class Bag : Item
{
    int weight;
    int Space;
}

public class Limb : Item
{
    int weight;
    int Space;
}
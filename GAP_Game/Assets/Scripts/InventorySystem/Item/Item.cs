using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class Item : ScriptableObject
{
    public string Name;
    public int ID;
    public int Health;
    public string PrefabAddress;
    public string IconAddress;
    public string Slot;

    public void Initialize(Item item, string slot)
    {
        Name = item.Name;
        ID = item.ID;
        Health = item.Health;
        PrefabAddress = item.PrefabAddress;
        IconAddress = item.IconAddress;
        Slot = slot;
    }

    public abstract void ActivateItem();
    public abstract void Deactivate();
}

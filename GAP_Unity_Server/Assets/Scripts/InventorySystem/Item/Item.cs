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
    [HideInInspector]
    public string Slot;

    public virtual void Initialize(Item item, string slot)
    {
        Name = item.Name;
        ID = item.ID;
        Health = item.Health;
        PrefabAddress = item.PrefabAddress;
        IconAddress = item.IconAddress;
        Slot = slot;
    }

    public void CreateFromNetCopy(NetItem item)
    {
        Name = item.Name;
        ID = item.ID;
        Health = item.Health;
        PrefabAddress = item.PrefabAddress;
        IconAddress = item.IconAddress;
        Slot = item.Slot;
    }

    public NetItem MakeNetCopy()
    {
        NetItem netCopy = new NetItem();
        netCopy.Inlitialize(this);
        return netCopy;
    }


    public abstract void ActivateItem();
    public abstract void Deactivate();
}


public class NetItem : NetEntity
{
    public string Name;
    public int ID;
    public int Health;
    public string PrefabAddress;
    public string IconAddress;
    [HideInInspector]
    public string Slot;

    public virtual void Inlitialize(Item item)
    {
        Name = item.Name;
        ID = item.ID;
        Health = item.Health;
        PrefabAddress = item.PrefabAddress;
        IconAddress = item.IconAddress;
        Slot = item.Slot;
    }
}
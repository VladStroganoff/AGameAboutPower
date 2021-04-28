using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class Item : ScriptableObject
{
    public string Name;
    public int ID;
    public int Health;
    public string Prefab; // should just be string
    public string Icon; // should just be string, we need to go full addressable for jsons sake;
    public abstract void ActivateItem();
    public abstract void Deactivate();
}

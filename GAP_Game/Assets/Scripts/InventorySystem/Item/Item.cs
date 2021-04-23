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


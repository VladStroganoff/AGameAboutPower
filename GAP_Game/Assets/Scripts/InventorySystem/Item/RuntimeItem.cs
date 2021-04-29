using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeItem
{
    public RuntimeItem(Item item)
    {
        Item = item;
    }

    public Item Item;
    public GameObject Prefab;
    public Sprite Icon;
}

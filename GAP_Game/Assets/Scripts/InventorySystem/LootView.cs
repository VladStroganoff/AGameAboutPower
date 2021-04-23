using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootView : MonoBehaviour, ILootController
{
    public RectTransform LootPanel;
    Vector2 Origin;

    public void ShowItems(List<Item> Items)
    {
    }
}

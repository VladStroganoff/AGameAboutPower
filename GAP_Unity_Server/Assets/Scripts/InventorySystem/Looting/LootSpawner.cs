#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LootSpawnType { Player, Food, Ammunition, Weapons, Random, Rare, Wearables }
public class LootSpawner : MonoBehaviour
{
    private List<Item> _loot = new List<Item>(); // just for debug
    public LootSpawnType Type = LootSpawnType.Random;
    LootController _lootControl;

    private void Awake()
    {
        _lootControl = GameObject.FindObjectOfType<LootController>();
        _lootControl.AddLootSpawner(this);
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        switch(Type)
        {
            case LootSpawnType.Player:
                Gizmos.DrawIcon(transform.position, "Player.png");
                gameObject.name = "Player-" + "ItemSpawner";
                return;
            case LootSpawnType.Food:
                Gizmos.DrawIcon(transform.position, "Food.png");
                gameObject.name = "Food-" + "ItemSpawner";
                return;
            case LootSpawnType.Ammunition:
                Gizmos.DrawIcon(transform.position, "Ammunition.png");
                gameObject.name = "Ammunition-" + "ItemSpawner";
                return;
            case LootSpawnType.Weapons:
                Gizmos.DrawIcon(transform.position, "Weapons.png");
                gameObject.name = "Weapons-" + "ItemSpawner";
                return;
            case LootSpawnType.Random:
                Gizmos.DrawIcon(transform.position, "Random.png");
                gameObject.name = "Random-" + "ItemSpawner";
                return;
            case LootSpawnType.Rare:
                Gizmos.DrawIcon(transform.position, "Rare.png");
                gameObject.name = "Rare-" + "ItemSpawner";
                return;
            case LootSpawnType.Wearables:
                Gizmos.DrawIcon(transform.position, "Wearables.png");
                gameObject.name = "Wearables-" + "ItemSpawner";
                return;
        }
    }
#endif

    public void SpawnLoot(List<Item> loot)
    {
        _loot = loot;
        GameObject lootGO = new GameObject();
        lootGO.transform.position = transform.position;
        LootView lootView = lootGO.AddComponent<LootView>();
        lootView.Initialize(loot, _lootControl);
    }

}

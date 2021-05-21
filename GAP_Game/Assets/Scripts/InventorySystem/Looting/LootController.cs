using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour, ILootController
{
    public Dictionary<int, LootView> Loot = new Dictionary<int, LootView>(); // for destroying when picked up
    public InventoryController InventoryControl;
    public void SpawnLoot(NetLoot netLoot)
    {
        GameObject lootGO = new GameObject();
        LootView lootView = lootGO.AddComponent<LootView>();
        lootView.Initialize(netLoot.GetItems(), netLoot.lootID);
        lootGO.transform.position = netLoot.Position;
        lootGO.transform.rotation = netLoot.Rotation;
        Loot.Add(netLoot.lootID, lootView);
    }

    public void DespawnLoot(int id)
    {
        Destroy(Loot[id]);
        Loot.Remove(id);
    }

    public void PickUpLoot(NetLoot loot)
    {
        InventoryControl.TakeItems(loot);
    }
}

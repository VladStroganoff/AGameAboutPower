using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour, ILootController
{
    public Dictionary<int, LootView> Loot = new Dictionary<int, LootView>(); // for destroying when picked up
    public InventoryController InventoryControl;
    public void SpawnLoot(NetLoot netLoot)
    {
        if (Loot.ContainsKey(netLoot.lootID))
        {
            UpdateLoot(netLoot);
            return;
        }
        GameObject lootGO = new GameObject();
        LootView lootView = lootGO.AddComponent<LootView>();
        lootView.Initialize(netLoot.GetItems(), netLoot.lootID);
        lootGO.transform.position = netLoot.Position;
        lootGO.transform.rotation = netLoot.Rotation;
        Loot.Add(netLoot.lootID, lootView);
    }

    void UpdateLoot(NetLoot netLoot)
    {
        //Loot[netLoot.lootID].Items = netLoot.GetItems();
    }

    public void DespawnLoot(int id)
    {
        Destroy(Loot[id].gameObject);
        Loot.Remove(id);
    }

    public void PickUpLoot(NetLoot loot)
    {
        InventoryControl.ShowLoot(loot);
    }
}

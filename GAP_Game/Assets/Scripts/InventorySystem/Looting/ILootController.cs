using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILootController
{
    void SpawnLoot(NetLoot loot);
    void DespawnLoot(int id);
    void PickUpLoot(NetLoot loot);
}
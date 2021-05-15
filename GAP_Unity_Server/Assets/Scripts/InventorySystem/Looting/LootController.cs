using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootController : MonoBehaviour // this controller is ment to feed the inventory system new loot and talk to database 
{
    

    public List<LootItemView> testLootItems = new List<LootItemView>();
    public bool testSveLoot;

    public DatabaseController DatabaseControl;
    private void Start()
    {
        if (testSveLoot)
            DatabaseControl.SaveWorldState(testLootItems);
    }

    public void InstansiateLoot(Vector3 position, Quaternion rotation, List<Item> items)
    {

    }

    public void LootPickedUp(NetLootItem items)
    {

    }
}

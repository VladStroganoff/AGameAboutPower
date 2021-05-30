using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : ItemReceiver
{
    Dictionary<int, InventoryModel> _playerInventories = new Dictionary<int, InventoryModel>();
    int focusPlayer = -1;

    public void AddPlayer(PlayerManager player)
    {
        player.PlayerDisconnect += RemovePlayer;
        _playerInventories.Add(player.ID, player.GetComponent<InventoryModel>());
    }

    public void RemovePlayer(PlayerManager player)
    {
        _playerInventories.Remove(player.ID);
    }

    public void TakeLoot(NetLoot newInventory)
    {
        List<Item> items = newInventory.GetItems();
        LoadController.instance.LoadRuntimeItems(items, this);
        focusPlayer = newInventory.ownerID;
    }


    public void ChangeWear(Netwearable netWear, int id)
    {
        Wearable wear = new Wearable();
        wear.Initialize(netWear);
        List<Item> items = new List<Item>();
        items.Add(wear as Item);

        if (netWear.Slot == "Head_Slot" || netWear.Slot == "Torso_Slot" || 
           netWear.Slot == "Legs_Slot" || netWear.Slot == "Right_Arm_Slot" || 
           netWear.Slot == "Left_Arm_Slot")
        {
            Debug.Log($"was in wear slot: {netWear.Slot}");
            LoadController.instance.LoadRuntimeItems(items, _playerInventories[id].GetComponent<DressController>()); // items recieved in the Dress controller
            ServerSend.ChangeWearable(id, netWear);
            return;
        }

        LoadController.instance.LoadRuntimeItems(items, this); // send only the function instead of the class
        Debug.Log($"was NOT in wear slot: {netWear.Slot}");
    }

    void UpdateInventory(List<RuntimeItem> items)
    {
        if(focusPlayer != -1)
        {
            _playerInventories[focusPlayer].AddItemsToPlayer(items);
            ServerSend.UpdatePlayerInventory(focusPlayer, _playerInventories[focusPlayer].NetInventory);
        }
        else
            Debug.Log("focus player was -1");

        focusPlayer = -1;
    }


    public override void RunItemsLoaded(List<RuntimeItem> runtimeItems)
    {
        UpdateInventory(runtimeItems);
    }
}

//public class Class1
//{
//    public int Method1(string input)
//    {
//        //... do something
//        return 0;
//    }

//    public int Method2(string input)
//    {
//        //... do something different
//        return 1;
//    }

//    public bool RunTheMethod(Func<string, int> myMethodName)
//    {
//        //... do stuff
//        int i = myMethodName("My String");
//        //... do more stuff
//        return true;
//    }

//    public bool Test()
//    {
//        return RunTheMethod(Method1);
//    }
//}
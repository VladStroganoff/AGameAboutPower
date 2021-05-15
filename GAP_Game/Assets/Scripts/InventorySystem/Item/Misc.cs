using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "New Misc Item", menuName = "InventoryItem/Misc")]
public class Misc : Item
{
   
    public override void ActivateItem()
    {
    }

    public override void Deactivate()
    {
    }
}

public class NetMisc : NetItem
{

}
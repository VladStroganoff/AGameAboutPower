#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootItemView : MonoBehaviour
{

    public List<Item> Items = new List<Item>();
    GameObject _graphics;
    public BoxCollider Collider;
    LootController _lootControl;

    #region Example
#if UNITY_EDITOR
    public void OnValidate() // just to illustrate that is there is just one Loot item it is displayed as one in the world, otherwhise its displayed as a "chest"
    {
        if (Items.Count != 1)
            return;

        if (_graphics == null)
        {
            _graphics = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Items[0].PrefabAddress), Vector3.zero, Quaternion.identity, transform);
            _graphics.transform.localPosition = Vector3.zero;
        }
        if (Collider == null)
        {
            Collider = gameObject.AddComponent<BoxCollider>();
            Bounds bounds = _graphics.GetComponent<MeshFilter>().mesh.bounds;
            Collider.size = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);
        }
    }
#endif
    #endregion

    public void Inject(LootController lootControl)
    {
        _lootControl = lootControl;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PlayerManager>() != null)
            return;

        int ID = other.gameObject.GetComponent<PlayerManager>().id;
        PrepNetItemPickup(Items, ID);
    }


    void PrepNetItemPickup(List<Item> items, int id) // maybe this data prep shoud be done further up the line
    {
        NetLootItem netLoot = new NetLootItem();
        List<NetItem> netItems = new List<NetItem>();
        foreach(var item in items)
        {
            if (item is Holdable)
            {
                var netItem = item.MakeNetWear();
                netItems.Add(netItem);
            }
            if (item is Wearable)
            {
                var netItem = item.MakeNetWear();
                netItems.Add(netItem);
            }
            if (item is Consumable)
            {
                var netItem = item.MakeNetConsumable();
                netItems.Add(netItem);
            }
            if (item is Misc)
            {
                var netItem = item.MakeNetMisc();
                netItems.Add(netItem);
            }
        }
        netLoot.ID = id;
        netLoot.Items = netItems.ToArray();
        netLoot.Position = transform.position;
        netLoot.Rotation = transform.rotation;
        _lootControl.LootPickedUp(netLoot);
    }
}


public class NetLootItem : NetEntity
{
    public int ID; // if null item is placed in world otherwhise its for an player to open.
    public NetItem[] Items;
    public Vector3 Position;
    public Quaternion Rotation;
}
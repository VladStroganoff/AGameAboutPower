using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LootItemView : MonoBehaviour, ItemReceiver
{

    public List<RuntimeItem> Items = new List<RuntimeItem>();
    GameObject _graphics;
    BoxCollider _collider;
    Rigidbody _rigidBody;
    LootController _lootControl;
    public string CrateAddress = "Assets/Content/Character/Props/Containers/WhiteboxCrate/DefaultCreate.prefab";

    public void Initialize(List<Item> items) 
    {
        LoadController.instance.LoadRuntimeItem(items, this);
    }

    public void Populate(List<RuntimeItem> runItems)
    {
        Items = runItems;
        if (transform.childCount != 0)
            return;
        if (runItems.Count == 0)
            return;
        if (runItems[0] == null)
            return;
        if (runItems.Count == 1)
        {
            PopulateSingle(runItems);
            return;
        }
        if (runItems.Count > 1)
            StartCoroutine(LoadController.instance.LoadSingle(CrateAddress, this));
    }


    void PopulateChest(GameObject chest)
    {
        _graphics = Instantiate(chest, Vector3.zero, Quaternion.identity, transform);
        _graphics.transform.localPosition = chest.transform.position;
        gameObject.name = $"Loot Chest-{Items.Count}-Items";
        
        if (_rigidBody == null)
            _rigidBody = gameObject.AddComponent<Rigidbody>();

        if (_collider == null)
            _collider = gameObject.AddComponent<BoxCollider>();
        Bounds bounds = _graphics.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
        _collider.size = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);
    }
    void PopulateSingle(List<RuntimeItem> runItems)
    {
        _graphics = runItems[0].Prefab;
        gameObject.name = $"{Items[0].Item.Name}-Loot_Item";

        if (_collider == null)
            _collider = gameObject.AddComponent<BoxCollider>();

        Bounds bounds = new Bounds();

        if (_graphics.GetComponent<MeshFilter>() != null)
        {
            bounds = _graphics.GetComponent<MeshFilter>().sharedMesh.bounds;
            _graphics = Instantiate(_graphics, transform);
            _graphics.transform.localPosition = -_graphics.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center;
        }
        else if (_graphics.GetComponentInChildren<MeshFilter>() != null)
        {
            bounds = _graphics.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
            _graphics = Instantiate(_graphics, transform);
            _graphics.transform.localPosition = -_graphics.GetComponentInChildren<MeshFilter>().sharedMesh.bounds.center;
        }
        if (_graphics.GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            bounds = MakeStaticItem(_graphics.GetComponentInChildren<SkinnedMeshRenderer>(), _graphics, Items[0].Item).GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
        }
        _collider.size = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);
        if (_rigidBody == null)
            _rigidBody = gameObject.AddComponent<Rigidbody>();
    }
    GameObject MakeStaticItem(SkinnedMeshRenderer skinnedMesh, GameObject oldObject, Item item) // maybe we will have special item for static representation but for now.
    {
        Mesh mesh = new Mesh();
        skinnedMesh.BakeMesh(mesh);
        Material[] materials = skinnedMesh.sharedMaterials;
        GameObject staticRepresentation = new GameObject($"{item.name}-Graphics");
        staticRepresentation.transform.SetParent(transform);
        staticRepresentation.transform.localPosition = -mesh.bounds.center;
        MeshFilter filter = staticRepresentation.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer rend = staticRepresentation.AddComponent<MeshRenderer>();
        rend.materials = materials;
        _graphics = staticRepresentation;
        return staticRepresentation;
    }


    public void Inject(LootController lootControl)
    {
        _lootControl = lootControl;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<PlayerManager>() == null)
            return;

        int ID = other.gameObject.GetComponent<PlayerManager>().ID;
        PrepNetItemPickup(Items, ID);
    }

   
    void PrepNetItemPickup(List<RuntimeItem> items, int id) // maybe this data prep shoud be done further up the line
    {
        NetLootItem netLoot = new NetLootItem();
        List<NetItem> netItems = new List<NetItem>();
        foreach (var item in items)
        {
            if (item is Holdable)
            {
                var netItem = item.Item.MakeNetWear();
                netItems.Add(netItem);
            }
            if (item is Wearable)
            {
                var netItem = item.Item.MakeNetWear();
                netItems.Add(netItem);
            }
            if (item is Consumable)
            {
                var netItem = item.Item.MakeNetConsumable();
                netItems.Add(netItem);
            }
            if (item is Misc)
            {
                var netItem = item.Item.MakeNetMisc();
                netItems.Add(netItem);
            }
        }
        netLoot.ID = id;
        netLoot.Items = netItems.ToArray();
        netLoot.Position = transform.position;
        netLoot.Rotation = transform.rotation;
        _lootControl.LootPickedUp(netLoot);
    }

    public void RecieveItem(GameObject loadedObject)
    {
        PopulateChest(loadedObject);
    }
}


public class NetLootItem : NetEntity
{
    public int ID; // if null item is placed in world otherwhise its for an player to open.
    public NetItem[] Items;
    public Vector3 Position;
    public Quaternion Rotation;
}
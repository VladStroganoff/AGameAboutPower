using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LootView : MonoBehaviour
{
    public int ID;
    public List<RuntimeItem> Items = new List<RuntimeItem>();
    GameObject _graphics;
    BoxCollider _collider;
    public string CrateAddress = "Assets/Content/Character/Props/Containers/RoughCrate/Crate.prefab";

    public void Initialize(List<Item> items, int id) 
    {
        LoadController.instance.LoadRuntimeItem(items, Populate);
        ID = id;
    }

    public void UpdateLoot(List<Item> items)
    {
        LoadController.instance.LoadRuntimeItem(items, Populate);
    }



    public int Populate(List<RuntimeItem> runItems)
    {
        Items = runItems;
        if (transform.childCount != 0)
            return 1;
        if (runItems.Count == 0)
            return 1;
        if (runItems[0] == null)
            return 1;
        if (runItems.Count == 1)
        {
            PopulateSingle(runItems);
            return 1;
        }
        if (runItems.Count > 1)
            StartCoroutine(LoadController.instance.LoadSingle(CrateAddress, RecieveItem));

        return 1;
    }
    void PopulateChest(GameObject chest)
    {
        _graphics = Instantiate(chest, Vector3.zero, Quaternion.identity, transform);
        _graphics.transform.localPosition = chest.transform.position;
        _graphics.transform.localRotation = Quaternion.Euler(new Vector3(0,90,0)); // for whitebox chest only


        gameObject.name = $"Loot Chest-{Items.Count}-Items";
        
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
    }
    GameObject MakeStaticItem(SkinnedMeshRenderer skinnedMesh, GameObject oldObject, Item item) // maybe we will have special item for static representation but for now.
    {
        Mesh mesh = new Mesh();
        skinnedMesh.BakeMesh(mesh);
        Material[] materials = skinnedMesh.sharedMaterials;
        GameObject staticRepresentation = new GameObject($"{item.name}-Graphics");
        staticRepresentation.transform.SetParent(transform);
        staticRepresentation.transform.localPosition = -mesh.bounds.center;
        staticRepresentation.transform.localRotation = Quaternion.identity;
        MeshFilter filter = staticRepresentation.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        MeshRenderer rend = staticRepresentation.AddComponent<MeshRenderer>();
        rend.materials = materials;
        _graphics = staticRepresentation;
        return staticRepresentation;
    }

    public int RecieveItem(GameObject loadedObject) // graphics from loader
    {
        PopulateChest(loadedObject);
        return 1;
    }
}


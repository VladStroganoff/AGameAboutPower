using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCombiner
{
    public readonly Dictionary<int, Transform> RootBoneDictionary = new Dictionary<int, Transform>();
    private Transform[] _boneTransforms = new Transform[57]; // total bone count

    public List<GameObject> TestLimb = new List<GameObject>();
    Transform root;

    public BoneCombiner(Transform rigg)
    {
        root = rigg;
        TraverseHierarchy(rigg);
        //foreach (GameObject limb in TestLimb)
        //{
        //    AddLimb(limb);
        //}
    }

    public Transform AddLimb(GameObject newLimb)
    {
        var limb = ProcessBoneObject(newLimb.GetComponentInChildren<SkinnedMeshRenderer>());
        limb.SetParent(root);
        return limb;
    }

    public void RemoveLimb(GameObject oldLimb)
    {

    }



    Transform ProcessBoneObject(SkinnedMeshRenderer renderer)
    {
        var bonedObject = new GameObject().transform;
        bonedObject.name = renderer.gameObject.name;
        bonedObject.transform.position = renderer.transform.position;
        bonedObject.transform.rotation = renderer.transform.rotation;
        
        var meshRend = bonedObject.gameObject.AddComponent<SkinnedMeshRenderer>();
        var bones = renderer.bones;

        for(int i =0; i < bones.Length; i++)
        {
            _boneTransforms[i] = RootBoneDictionary[bones[i].name.GetHashCode()];
        }

        meshRend.bones = _boneTransforms;
        meshRend.sharedMesh = renderer.sharedMesh;
        meshRend.materials = renderer.materials;
        
        return bonedObject;
    }

    void TraverseHierarchy(Transform root)
    {
        foreach(Transform child in root)
        {
            RootBoneDictionary.Add(child.name.GetHashCode(), child);
            TraverseHierarchy(child);
        }
    }

}

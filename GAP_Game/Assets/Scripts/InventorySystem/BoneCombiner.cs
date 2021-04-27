using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCombiner
{
    public readonly Dictionary<int, Transform> RootBoneDictionary = new Dictionary<int, Transform>();
    private Transform[] _boneTransforms = new Transform[57]; // total bone count in original rigg

    public List<GameObject> TestLimb = new List<GameObject>();
    Transform root;

    public BoneCombiner(Transform rigg)
    {
        root = rigg;
        TraverseHierarchy(rigg);
    }

    public Transform AddLimb(GameObject newLimb, List<string> boneNames)
    {
        var limb = ProcessBoneObject(newLimb.GetComponentInChildren<SkinnedMeshRenderer>(), boneNames);
        limb.SetParent(root);
        limb.position = Vector3.zero;
        limb.rotation = Quaternion.identity;
        return limb;
    }

    public void RemoveLimb(GameObject oldLimb)
    {
        GameObject.Destroy(oldLimb);
    }

    Transform ProcessBoneObject(SkinnedMeshRenderer renderer, List<string> boneNames)
    {
        var bonedObject = new GameObject();
        bonedObject.name = renderer.gameObject.name;
        bonedObject.name = bonedObject.name.Replace("(Clone)", "");
        bonedObject.transform.position = renderer.transform.position;
        bonedObject.transform.rotation = renderer.transform.rotation;
        
        var meshRend = bonedObject.gameObject.AddComponent<SkinnedMeshRenderer>();

        for(int i =0; i < boneNames.Count; i++)
        {
            if (RootBoneDictionary.ContainsKey(boneNames[i].GetHashCode()))
                _boneTransforms[i] = RootBoneDictionary[boneNames[i].GetHashCode()];
            else
                Debug.Log($"Bone: {boneNames[i]} is not in Dictionary");
        }

        meshRend.bones = _boneTransforms;
        meshRend.sharedMesh = renderer.sharedMesh;
        meshRend.sharedMaterials = renderer.sharedMaterials;

        return bonedObject.transform;
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

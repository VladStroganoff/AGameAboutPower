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

    public Transform AddLimb(GameObject newLimb)
    {
        var limb = ProcessBoneObject(newLimb.GetComponentInChildren<SkinnedMeshRenderer>());
        limb.SetParent(root);
        limb.position = Vector3.zero;
        limb.rotation = Quaternion.identity;
        return limb;
    }

    public void RemoveLimb(GameObject oldLimb)
    {
        GameObject.Destroy(oldLimb);
    }

    public void PrepWearable(RuntimeItem wearable)
    {

    }

    Transform ProcessBoneObject(SkinnedMeshRenderer renderer)
    {
        var bonedObject = new GameObject();
        bonedObject.name = renderer.gameObject.name;
        bonedObject.name = bonedObject.name.Replace("(Clone)", "");
        bonedObject.transform.position = renderer.transform.position;
        bonedObject.transform.rotation = renderer.transform.rotation;
        
        var meshRend = bonedObject.gameObject.AddComponent<SkinnedMeshRenderer>();
        var bones = renderer.bones;
        for(int i =0; i < bones.Length; i++)
        {
            if (RootBoneDictionary.ContainsKey(bones[i].name.GetHashCode()))
                _boneTransforms[i] = RootBoneDictionary[bones[i].name.GetHashCode()];
            else
                Debug.Log($"Bone: {bones[i].name} is not in Dictionary");
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

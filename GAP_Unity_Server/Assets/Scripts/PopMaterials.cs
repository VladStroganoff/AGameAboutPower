using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PopMaterials : MonoBehaviour
{
    public bool DoIt;
    public Material putMaterial;

    void Update()
    {
        if(DoIt)
        {
            DoIt = false;
            PopulateMaterials();
            DestroyImmediate(this);
        }
    }

    void PopulateMaterials()
    {
        Terrain[] terras = GetComponentsInChildren<Terrain>();
        for(int i = 0; i < terras.Length; i++)
        {
            terras[i].materialTemplate = putMaterial;
        }

    }

}

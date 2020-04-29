using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.isReadable = true;
    }
}

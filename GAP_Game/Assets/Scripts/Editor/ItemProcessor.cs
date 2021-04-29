using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        for (var index = 0; index < importedAssets.Length; index++)
        {
            var importedAsset = importedAssets[index];

            var asset = AssetDatabase.LoadAssetAtPath<Object>(importedAsset);

            if (asset is Item)
            {
                var itemData = (Item)asset;
                itemData.ID = Mathf.Abs(asset.GetHashCode());
                itemData.Health = 100;
            }
        }
    }
}


using UnityEngine;
using UnityEditor;
using System.Collections;
using System;


public class TexturePacker : MonoBehaviour
{
    public Texture2D[] Textures;
    public string SaveLocation = "Assets/TextureArray.tarr";

    public void CreateTextureArray()
    {
        Texture2DArray array = new Texture2DArray(Textures[0].width, Textures[0].height, Textures.Length, Textures[0].format, false);


        for (int i = 0; i < Textures.Length; i++)
            array.SetPixels(Textures[i].GetPixels(), i);

        array.Apply();
        AssetDatabase.CreateAsset(array, SaveLocation);
    }
}



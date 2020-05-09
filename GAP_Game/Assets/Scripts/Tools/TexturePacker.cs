using UnityEngine;
using UnityEditor;
using System.Collections;
using System;


public class TexturePacker : MonoBehaviour
{

    public string AlbedoTextureURL = "Assets/Content/Enviroment/Landscaping/AlbedosAndComp.asset";

    public Texture2D[] AlbedoTextures;

    public void CreateTextureArray()
    {
        Texture2DArray array = new Texture2DArray(AlbedoTextures[0].width, AlbedoTextures[0].height, AlbedoTextures.Length, AlbedoTextures[0].format, false);


        for (int i = 0; i < AlbedoTextures.Length; i++)
            array.SetPixels(AlbedoTextures[i].GetPixels(), i);

        array.Apply();
        AssetDatabase.CreateAsset(array, AlbedoTextureURL);
    }
}



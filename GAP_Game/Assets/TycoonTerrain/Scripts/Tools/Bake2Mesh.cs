using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Bake2Mesh : MonoBehaviour
{
    GameObject TempGraphics;
    GameObject Graphics;
    public TycoonTileRenderer MapRenderer;
    public bool DebugMode;


    public void GenerateMeshOfMap()
    {
        TempGraphics = new GameObject("Temp");
        Graphics = new GameObject("Graphics");
        MapRenderer.GenerateMesh(TempGraphics.transform);
        foreach (Transform child in TempGraphics.transform)
        {
            if (child.GetComponent<MeshFilter>().mesh.triangles.Length > 1)
                SaveMesh(child.gameObject, child.GetComponent<MeshFilter>().mesh, child.name);

            if (DebugMode)
            {
                Debug.Log("Mesh debug is on.");
                break;
            }
        }

        AssetDatabase.SaveAssets();

        Destroy(TempGraphics);
    }

    void SaveMesh(GameObject go, Mesh _mesh, string name)
    {
        if (!File.Exists(Application.dataPath + "/TycoonTerrain/BakedTerrain/Meshes/" + "mesh_" + name + ".obj"))
            ObjExporter.MeshToFile(_mesh, go.GetComponent<MeshRenderer>().sharedMaterials, Application.dataPath + "/TycoonTerrain/BakedTerrain/Meshes/" + "mesh_" + name + ".obj");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var fullObj = new GameObject(name);

        if (go.GetComponent<MeshRenderer>() != null)
        {
            fullObj.transform.SetParent(Graphics.transform);
            fullObj.transform.position = go.transform.position;
            fullObj.transform.rotation = Quaternion.Euler(0, -90, 0);
            fullObj.AddComponent<MeshFilter>();
            fullObj.GetComponent<MeshFilter>().mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/TycoonTerrain/BakedTerrain/Meshes/" + "mesh_" + name + ".obj");
            fullObj.AddComponent<MeshRenderer>();
            fullObj.GetComponent<MeshRenderer>().material = go.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}

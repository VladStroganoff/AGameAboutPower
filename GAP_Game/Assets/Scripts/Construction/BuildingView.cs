using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingView : MonoBehaviour
{
    [SerializeField]
    private BuildingData Data;
    public BuildingData GetData()
    {
        Data.Position = transform.position;
        Data.Rotation = transform.rotation;
        return Data;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingType { Farm, Barrack, Tower, Wall, Gate}


[System.Serializable]
public struct BuildingData
{
    public string Name;
    public int Cost;
    public Vector3 Position;
    public Quaternion Rotation;
}

using UnityEngine;

public enum BuildingType { Farm, Barrack, Tower, Wall, Gate }


[System.Serializable]
public struct BuildingData
{
    [SerializeField]
    public string Name;
    [SerializeField]
    public int Cost;
    [SerializeField]
    public Vector3 Position;
    [SerializeField]
    public Quaternion Rotation;
}

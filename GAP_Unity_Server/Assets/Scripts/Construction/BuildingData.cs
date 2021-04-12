using UnityEngine;

public enum BuildingType { Farm, Barrack, Tower, Wall, Gate }


[System.Serializable]
public class BuildingData : NetEntity
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

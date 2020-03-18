using UnityEngine;

[System.Serializable]
public class NetWorld
{
    public NetLandTile[,] LandTiles;
}

[System.Serializable]
public class NetLandTile
{
    [SerializeField] public byte cornerNE;
    [SerializeField] public byte cornerSE;
    [SerializeField] public byte cornerSW;
    [SerializeField] public byte cornerNW;
    [SerializeField] public ushort waterHeight;

    [SerializeField] public ushort tileType;
}




[System.Serializable]
public struct NetworkedTransform : IComponent
{
    public SVector3 position;
    public SQuaternion rotation;
}


[System.Serializable]
public struct SQuaternion
{
    public float w;
    public float x;
    public float y;
    public float z;
}



[System.Serializable]
public struct SVector3
{
    public float x;
    public float y;
    public float z;
}

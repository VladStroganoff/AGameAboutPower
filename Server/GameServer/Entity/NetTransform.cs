
    [System.Serializable]
    public class NetTransform : NetComponent
    {
        public SVector3 position;
        public SQuaternion rotation;
    }


    public struct SQuaternion
    {
        public float w;
        public float x;
        public float y;
        public float z;
    }


    public struct SVector3
    {
        public float x;
        public float y;
        public float z;


    }


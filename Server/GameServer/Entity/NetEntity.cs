
    [System.Serializable]
    public class NetEntity
    {
        public int ConnectionID;
        public int ID;
        public bool Online;
        public NetComponent[] Components;
    }

    [System.Serializable]
    public class NetComponent
    {
    }


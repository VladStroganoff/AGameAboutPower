using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetEntity
{
    public int ConnectionID;
    public bool Online;
    public NetComponent[] Components; 
}

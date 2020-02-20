using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkedEntity
{
    public int ConnectionID;
    public bool Online;
    public IComponent[] Components; 
}

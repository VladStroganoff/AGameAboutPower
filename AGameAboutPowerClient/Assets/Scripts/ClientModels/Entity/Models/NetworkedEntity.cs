using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkedEntity
{
    public int ConnectionID;
    public string PrefabName;
    public bool Online;
    public NetworkedTransform SpawnTransform;
    public NetworkedCustomData CustomData;
    public NetworkedTransform Transform;
    public NetworkedAnimator Animatior;

    public NetworkedEntity(NetworkedCustomData data)
    {
        CustomData = data;
    }
}

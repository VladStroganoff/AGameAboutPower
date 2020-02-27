using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public PlayerController PlayerControl;

    public void HandleEntity(NetEntity entity)
    {
        if(entity.Components == null)
        {
            FDebug.Log.Message("Components where null on player" + entity.ConnectionID);
            return;
        }

        foreach(NetComponent component in entity.Components)

        if(component is PlayerData)
        {
            PlayerControl.HandlePlayer(entity);
        }
        else
        {

        }

    }

}

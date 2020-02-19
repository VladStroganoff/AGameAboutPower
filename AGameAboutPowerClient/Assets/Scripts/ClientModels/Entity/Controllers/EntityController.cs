using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public PlayerController PlayerControl;

    public void HandleEntity(NetworkedEntity entity)
    {
        if(entity.CustomData is PlayerData)
        {
            PlayerControl.HandlePlayer(entity);
        }
        else
        {

        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public PlayerController PlayerControl;

    public void HandleEntity(NetworkedEntity entity)
    {
        foreach(IComponent component in entity.Components)

        if(component is PlayerData)
        {
            PlayerControl.HandlePlayer(entity);
        }
        else
        {

        }

    }

}

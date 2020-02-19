using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MakeEntity 
{

    public static NetworkedEntity NewPlyer(Transform transform, int ID, string name)
    {
        NetworkedEntity entity = new NetworkedEntity(new PlayerData("Player TestName"));
        entity.ConnectionID = ID;
        entity.Transform.position.x = transform.GetChild(0).position.x;
        entity.Transform.position.y = transform.GetChild(0).position.y;
        entity.Transform.position.z = transform.GetChild(0).position.z;

        entity.Transform.rotation.x = transform.GetChild(0).rotation.x;
        entity.Transform.rotation.y = transform.GetChild(0).rotation.y;
        entity.Transform.rotation.z = transform.GetChild(0).rotation.z;
        entity.Transform.rotation.w = transform.GetChild(0).rotation.w;



        return entity;
    }

    public static NetworkedEntity PlayerUpdate(NetworkedEntity data, Transform transform)
    {
        data.Transform.position.x = transform.GetChild(0).position.x;
        data.Transform.position.y = transform.GetChild(0).position.y;
        data.Transform.position.z = transform.GetChild(0).position.z;

        data.Transform.rotation.x = transform.GetChild(0).rotation.x;
        data.Transform.rotation.y = transform.GetChild(0).rotation.y;
        data.Transform.rotation.z = transform.GetChild(0).rotation.z;
        data.Transform.rotation.w = transform.GetChild(0).rotation.w;

        return data;
    }

}

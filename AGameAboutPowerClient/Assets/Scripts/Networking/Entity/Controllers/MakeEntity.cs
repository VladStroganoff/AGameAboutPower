using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MakeEntity
{

    public static NetEntity NewEntity(int ID)
    {
        NetEntity entity = new NetEntity();
        entity.Online = true;

        return entity;
    }

    public static void AddComponent(NetEntity entity, NetComponent component)
    {

        AddOneComponent(entity);
        entity.Components[entity.Components.Length - 1] = component;
    }

    public static T GetComponent<T>(NetEntity entity) where T : NetComponent
    {

        if (typeof(T) == null)
            return null;

        foreach (NetComponent component in entity.Components)
        {
            T requestedType = component as T;

            if (requestedType != null)
                return requestedType;
        }

        return null;

    }



    public static NetEntity AddTransformComponent(NetEntity entity, NetTransform transform)
    {
        AddOneComponent(entity);

        NetTransform temp = new NetTransform();

        temp.position.x = transform.position.x;
        temp.position.y = transform.position.y;
        temp.position.z = transform.position.z;

        temp.rotation.x = transform.rotation.x;
        temp.rotation.y = transform.rotation.y;
        temp.rotation.z = transform.rotation.z;
        temp.rotation.w = transform.rotation.w;

        entity.Components[entity.Components.Length] = temp;

        return entity;
    }

    public static NetEntity UpdateTransform(NetEntity entity, Transform transform)
    {
        bool foundTransform = false;
        foreach(NetComponent comp in entity.Components)
        {
            if(comp is NetTransform)
            {
                foundTransform = true;
            }
        }


        if (!foundTransform)
            AddOneComponent(entity);




        for (int i = 0; i < entity.Components.Length; i++)
        {
            if (entity.Components[i] is NetTransform || entity.Components[i] == null)
            {
                NetTransform temp = new NetTransform();

                temp.position.x = transform.position.x;
                temp.position.y = transform.position.y;
                temp.position.z = transform.position.z;

                temp.rotation.x = transform.rotation.x;
                temp.rotation.y = transform.rotation.y;
                temp.rotation.z = transform.rotation.z;
                temp.rotation.w = transform.rotation.w;

                entity.Components[i] = temp;
            }
        }



        return entity;
    }

    static void AddOneComponent(NetEntity entity)
    {
        if (entity.Components == null)
        {
            entity.Components = new NetComponent[1];
            return;
        }

        NetComponent[] components = new NetComponent[entity.Components.Length + 1];

        System.Array.Copy(entity.Components, 0, components, 0, entity.Components.Length);

        entity.Components = components;
    }
}


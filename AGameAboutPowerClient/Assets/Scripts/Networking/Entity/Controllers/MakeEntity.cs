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

        System.Random rnd = new System.Random();
        entity.ID = rnd.Next(11111, 99999);

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

    public static NetEntity UpdateAnimator(NetEntity entity, Animator animator)
    {

        bool foundTransform = false;
        foreach (NetComponent comp in entity.Components)
        {
            if (comp is NetAnimator)
            {
                foundTransform = true;
            }
        }


        if (!foundTransform)
            AddOneComponent(entity);

        for (int i = 0; i < entity.Components.Length; i++)
        {
            if (entity.Components[i] is NetAnimator || entity.Components[i] == null)
            {
                NetAnimator temp = new NetAnimator();
                temp.Parameters = new NetAnimatorComponent[animator.parameters.Length];

                for(int j =0; j < animator.parameters.Length; j++)
                {
                    switch (animator.parameters[j].type)
                    {
                        case AnimatorControllerParameterType.Bool:
                            {
                                NetAnimatorBool animBool = new NetAnimatorBool();
                                animBool.name = animator.parameters[j].name;
                                animBool.state = animator.GetBool(animator.parameters[j].name);
                                temp.Parameters[j] = animBool;
                                continue;
                            }
                        case AnimatorControllerParameterType.Float:
                            {
                                NetAnimatorFloat animfloat = new NetAnimatorFloat();
                                animfloat.name = animator.parameters[j].name;
                                animfloat.value = animator.GetFloat(animator.parameters[j].name) ;
                                temp.Parameters[j] = animfloat;
                                continue;
                            }
                        case AnimatorControllerParameterType.Trigger:
                            {
                                NetAnimatorTrigger animtrigger = new NetAnimatorTrigger();
                                animtrigger.name = animator.parameters[j].name;
                                animtrigger.state = animator.parameters[j].defaultBool;
                                temp.Parameters[j] = animtrigger;
                                continue;
                            }
                        case AnimatorControllerParameterType.Int:
                            {
                                NetAnimatorInt animint = new NetAnimatorInt();
                                animint.name = animator.parameters[j].name;
                                animint.value = animator.GetInteger(animator.parameters[j].name);
                                temp.Parameters[j] = animint;
                                continue;
                            }
                    }

                }
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


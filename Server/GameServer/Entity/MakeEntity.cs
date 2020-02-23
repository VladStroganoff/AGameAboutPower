using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entity
{
    public static class MakeEntity
    {

        public static NetworkedEntity NewEntity(int ID)
        {
            NetworkedEntity entity = new NetworkedEntity();
            entity.Online = true;

            return entity;
        }

        public static void AddComponent(NetworkedEntity entity, IComponent component)
        {

            AddOneComponent(entity);
            entity.Components[entity.Components.Length] = component;
        }

        public static IComponent GetComponent <T>(NetworkedEntity entity, T request) where T : class, IComponent
        {
            if (T == null)
                return null;

            if(request is PlayerData)
            {
             foreach(IComponent component in entity.Components)
                {
                    if(component is PlayerData)
                    {
                        return component;
                    }
                }
            }

            if (request is NetworkedTransform)
            {
                foreach (IComponent component in entity.Components)
                {
                    if (component is NetworkedTransform)
                    {
                        return component;
                    }
                }
            }

            if (request is NetworkedAnimator)
            {
                foreach (IComponent component in entity.Components)
                {
                    if (component is NetworkedAnimator)
                    {
                        return component;
                    }
                }
            }

            return null;

        }



        public static NetworkedEntity AddTransformComponent(NetworkedEntity entity, NetworkedTransform transform)
        {
            AddOneComponent(entity);

            NetworkedTransform temp = new NetworkedTransform();

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

        public static NetworkedTransform GetTransform(NetworkedEntity entity)
        {
            foreach (IComponent component in entity.Components)
            {
                if (component is NetworkedTransform)
                {
                    return (NetworkedTransform)component;
                }

            }


            return new NetworkedTransform();

        }

        public static NetworkedEntity UpdateTransform(NetworkedEntity entity, NetworkedTransform transform)
        {
            for (int i = 0; i < entity.Components.Length; i++)
            {
                if (entity.Components[i] is NetworkedTransform)
                {
                    NetworkedTransform temp = (NetworkedTransform)entity.Components[i];

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

        static void AddOneComponent(NetworkedEntity entity)
        {
            IComponent[] components = new IComponent[entity.Components.Length + 1];

            System.Array.Copy(entity.Components, 0, components, 0, entity.Components.Length);

            entity.Components = components;
        }
    }

}

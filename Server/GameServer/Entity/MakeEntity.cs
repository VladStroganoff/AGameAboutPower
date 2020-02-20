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

        public static NetworkedEntity AddPlayerComponent(NetworkedEntity entity, string prefabName)
        {

            entity = AddOneComponent(entity);

            PlayerData data = new PlayerData();

            data.PrefabName = prefabName;
            data.Name = "Player Name";

            entity.Components[entity.Components.Length] = data;

            return entity;
        }

        public static NetworkedEntity AddTransformComponent(NetworkedEntity entity, NetworkedTransform transform)
        {
            entity = AddOneComponent(entity);

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

        static NetworkedEntity AddOneComponent(NetworkedEntity entity)
        {
            IComponent[] components = new IComponent[entity.Components.Length + 1];

            System.Array.Copy(entity.Components, 0, components, 0, entity.Components.Length);

            entity.Components = components;

            return entity;
        }
    }

}

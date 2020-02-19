using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entity
{
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
            CustomData = data; // if Im converting this into ECS maybe this will go eventually. 
        }
    }
}

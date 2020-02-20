using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entity
{
    public struct NetworkedTransform : IComponent
    {
        public SVector3 position;
        public SQuaternion rotation;
    }


    public struct SQuaternion
    {
        public float w;
        public float x;
        public float y;
        public float z;
    }


    public struct SVector3
    {
        public float x;
        public float y;
        public float z;


    }

}

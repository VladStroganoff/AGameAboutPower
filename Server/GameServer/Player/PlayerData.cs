using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Player
{
    public class PlayerData
    {
        public int ConnectionID = 0;
        public string Name = "";
        public bool Online = true;
        public SVector3 position = new SVector3();
        public SQuaternion rotation = new SQuaternion();
    }
}

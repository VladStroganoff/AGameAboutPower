using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entity
{
    public class PlayerData : NetworkedCustomData
    {
        public string Name = "";

        public PlayerData(string name)
        {
            Name = name;
        }
    }
}

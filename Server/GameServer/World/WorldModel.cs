﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Player;

namespace GameServer.World
{
    public class WorldModel
    {
        public Dictionary<int, PlayerData> Players = new Dictionary<int, PlayerData>();
    }
}
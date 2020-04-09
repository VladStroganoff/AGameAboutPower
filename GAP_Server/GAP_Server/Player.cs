using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GAP_Server
{
    class Player
    {
        public int ID;
        public string name;
        public Vector3 position;
        public Quaternion rotation;

        public Player(int id, string username, Vector3 spawnpoint)
        {
            ID = id;
            name = username;
            position = spawnpoint;
        }
    }
}

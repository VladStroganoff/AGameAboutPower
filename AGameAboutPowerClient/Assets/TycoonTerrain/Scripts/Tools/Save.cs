using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TycoonTerrain.Core
{
    public class Save : MonoBehaviour
    {
        public TycoonTileMap map;




        public void SaveMap()
        {
            //foreach(map.Grid.map)
        }

    }

    public class SaveWorld
    {
        SaveWorld[,] World;

        public SaveWorld(int size)
        {
            World = new SaveWorld[size, size];
        }

    }

    public class SaveTile
    {
        LandTile height;
        int type;
    }

}


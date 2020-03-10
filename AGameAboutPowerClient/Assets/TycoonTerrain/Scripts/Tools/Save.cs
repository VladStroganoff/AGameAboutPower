using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


namespace TycoonTerrain.Core
{
    public class Save : MonoBehaviour
    {
        public TycoonTileMap map;

        SaveWorld save;
        public TerrainGrid MapGrid;
        public TerrainTypeTable MapTypes;

       


        public void SaveMap()
        {
            MapGrid = map.GetGrid();
            MapTypes = map.TypeTable;

            save = new SaveWorld();


            for (int i =0; i< MapGrid.Length; i++)
            {
                for (int j = 0; j < MapGrid.Width; j++)
                {
                    int2 pos = new int2(i, j);
                    TileHandle tile = MapGrid.GetTile(pos);
                    LandTile data = tile.GetData();

                    save.tiles.Add(pos, data);
                    save.tileTypes.Add(pos, MapTypes.GetTerrainType(pos));
                }
            }


            Debug.Log("yall whaaat?");
        }

    }

    public class SaveWorld
    {
        public Dictionary<int2, LandTile> tiles = new Dictionary<int2, LandTile>();
        public Dictionary<int2, ushort> tileTypes = new Dictionary<int2, ushort>();
    }
}


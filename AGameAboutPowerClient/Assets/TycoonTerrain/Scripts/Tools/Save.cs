using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;


namespace TycoonTerrain.Core
{
    public class Save : MonoBehaviour
    {
        public TycoonTileMap map;

        NetWorld save;
        public TerrainGrid MapGrid;
        public TerrainTypeTable MapTypes;
        public string url;

        public Dictionary<int2, LandTile> tiles = new Dictionary<int2, LandTile>();
        public Dictionary<int2, ushort> tileTypes = new Dictionary<int2, ushort>();

        public void SaveMap()
        {
            MapGrid = map.GetGrid();
            MapTypes = map.TypeTable;

            save = new NetWorld();
            int2 pos = new int2(0, 0);

            for (int i =0; i< MapGrid.Length; i++)
            {
                for (int j = 0; j < MapGrid.Width; j++)
                {
                    pos = new int2(i, j);
                    TileHandle tile = MapGrid.GetTile(pos);
                    LandTile data = tile.GetData();


                    if (tiles.ContainsKey(pos))
                        Debug.Log("kuken?");


                    tiles.Add(pos, data);
                    tileTypes.Add(pos, MapTypes.GetTerrainType(pos));
                }
            }

            save.LandTiles = new NetLandTile[pos.x, pos.y];

            for(int i = 0; i < pos.x; i++)
            {
                for(int j = 0; j < pos.y; j++)
                {
                    int2 key = new int2(i, j);
                    NetLandTile theTile = new NetLandTile();

                    theTile.cornerNE = tiles[key].GetHeight(CornerIndex.NorthEast);
                    theTile.cornerSE = tiles[key].GetHeight(CornerIndex.SouthEast);
                    theTile.cornerSW = tiles[key].GetHeight(CornerIndex.SouthWest);
                    theTile.cornerNW = tiles[key].GetHeight(CornerIndex.NorthWest);
                    theTile.waterHeight = tiles[key].WaterLevel;
                    theTile.tileType = MapTypes.GetTerrainType(key);

                    save.LandTiles[i, j] = theTile;
                }
            }

            string json = JsonConvert.SerializeObject(save);
            File.WriteAllText(url, json);

            Debug.Log("ye saved mkay. at: " + url);

            tiles = new Dictionary<int2, LandTile>();
            tileTypes = new Dictionary<int2, ushort>();
        }

    }

    [System.Serializable]
    public class NetWorld
    {
        public NetLandTile[,] LandTiles;
    }

    public class NetLandTile
    {
        [SerializeField] public byte cornerNE;
        [SerializeField] public byte cornerSE;
        [SerializeField] public byte cornerSW;
        [SerializeField] public byte cornerNW;
        [SerializeField] public ushort waterHeight;

        [SerializeField] public ushort tileType;
    }

}


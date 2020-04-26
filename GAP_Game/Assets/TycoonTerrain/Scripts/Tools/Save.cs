using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TycoonTerrain.Core.TerrainOperations;
using TycoonTerrain.Core.Generation;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


namespace TycoonTerrain.Core
{
    public class Save : MonoBehaviour
    {
        public TycoonTileMap TycoonMap;
        public TerrainGrid MapGrid;
        public TerrainTypeTable MapTypes;
        //public string url;

        public bool LoadOnStart;
        public TycoonTileRenderer MapRenderer;
        public GameObject WorldOrigin;
        private NetWorld save;
        private NativeArray<byte> heightData;
        public Dictionary<int2, LandTile> tiles = new Dictionary<int2, LandTile>();
        public Dictionary<int2, ushort> tileTypes = new Dictionary<int2, ushort>();


        public void Start()
        {
            if(LoadOnStart)
            {
                LoadMap();
            }
        }

        public void SaveMap()
        {
            MapGrid = TycoonMap.GetGrid();
            MapTypes = TycoonMap.TypeTable;

            save = new NetWorld();
            int2 pos = new int2(0, 0);

            for (int i = 0; i < MapGrid.Length; i++)
            {
                for (int j = 0; j < MapGrid.Width; j++)
                {
                    pos = new int2(i, j);
                    TileHandle tile = MapGrid.GetTile(pos);
                    LandTile data = tile.GetData();


                    tiles.Add(pos, data);
                    tileTypes.Add(pos, MapTypes.GetTerrainType(pos));
                }
            }

            save.LandTiles = new NetLandTile[pos.x, pos.y];

            for (int i = 0; i < pos.x; i++)
            {
                for (int j = 0; j < pos.y; j++)
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
            File.WriteAllText(Application.persistentDataPath + "/map.save", json);

            Debug.Log("ye saved mkay. at: " + Application.persistentDataPath + "/map.save");

            tiles = new Dictionary<int2, LandTile>();
            tileTypes = new Dictionary<int2, ushort>();
        }

        public void LoadMap()
        {
            if (!File.Exists(Application.persistentDataPath + "/map.save"))
            {
                FDebug.Log.Message("could not find file at: " + Application.persistentDataPath + "/map.save");
                return;
            }

            string text = File.ReadAllText(Application.persistentDataPath + "/map.save");

            NetWorld worldSave = JsonConvert.DeserializeObject<NetWorld>(text);


            TycoonMap.ScheduleOperation(new LoadTerrainOperation(worldSave.LandTiles));
            TycoonMap.SchedulePaintOperation(new BeachLoadOperation(worldSave.LandTiles));
            int2 waterSetPosition = new int2(81, 89);
            TycoonMap.ScheduleOperation(new CreateWaterBodyFloodOperation(waterSetPosition, 26, TycoonMap.WaterHeightStepsPerTileHeight));

        }


        public void GenerateMeshOfMap()
        {
            WorldOrigin = new GameObject("World");
            MapRenderer.GenerateMesh(WorldOrigin.transform);
        }

    }


}




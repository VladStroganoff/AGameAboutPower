using System.Collections;
using System.Collections.Generic;
using TycoonTerrain.Core;
using TycoonTerrain.Core.TerrainOperations;
using Unity.Mathematics;
using UnityEngine;

public class Pixel2DataTool : MonoBehaviour
{
    public TycoonTileMap TycoonMap;
    public Texture2D HeightMap;
    NetWorld world;
    int2 tilePos;

    public void LoadUpPixels()
    {
        tilePos = new int2();
        world = new NetWorld(TycoonMap.Length, TycoonMap.Width);

        for(int x =0; x< HeightMap.width; x= (x+2)) // I capture the pixels in groups of four, to match the tiles
        {
            if(x!=0)
                tilePos.x++;

            for(int y = 0; y < HeightMap.height; y = (y+2))
            {
                if (y != 0)
                    tilePos.y++;

                float cornerSW = HeightMap.GetPixel(x+1, y).a * 255f;
                float cornerSE = HeightMap.GetPixel(x, y).a * 255f;
                float cornerNW = HeightMap.GetPixel(x+1, y+1).a * 255f;
                float cornerNE = HeightMap.GetPixel(x, y+1).a * 255f;

                NetLandTile tile = new NetLandTile();
                tile.cornerSW = (byte)cornerSW;
                tile.cornerSE = (byte)cornerSE;
                tile.cornerNW = (byte)cornerNW;
                tile.cornerNE = (byte)cornerNE;

                if (tilePos.y > 1020)
                    Debug.Log("yo");

                world.LandTiles[tilePos.x, tilePos.y] = tile;

            }
            tilePos.y = 0;
        }


        TycoonMap.ScheduleOperation(new LoadTerrainOperation(world.LandTiles));
    }
}

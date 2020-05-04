using System.Collections;
using System.Collections.Generic;
using TycoonTerrain.Core;
using TycoonTerrain.Core.TerrainOperations;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;

public class Pixel2DataTool : MonoBehaviour
{
    public TycoonTileMap TycoonMap;
    public Texture2D HeightMap;
    public Texture2D BasicCoveridgeMap;
    NetWorld world;
    int2 tilePos;

    Dictionary<string, int> RGBA = new Dictionary<string, int>();

    public void LoadUpPixels()
    {
        tilePos = new int2();
        world = new NetWorld(TycoonMap.Length, TycoonMap.Width);

        for (int x = 0; x < HeightMap.width; x = (x + 2)) // I capture the pixels in groups of four, to match the tiles
        {
            if (x != 0)
                tilePos.x++;

            for (int y = 0; y < HeightMap.height; y = (y + 2))
            {
                if (y != 0)
                    tilePos.y++;



                float cornerSW = HeightMap.GetPixel(x + 1, y).r * 255f;
                float cornerSE = HeightMap.GetPixel(x, y).r * 255f;
                float cornerNW = HeightMap.GetPixel(x + 1, y + 1).r * 255f;
                float cornerNE = HeightMap.GetPixel(x, y + 1).r * 255f;

                NetLandTile tile = new NetLandTile();
                tile.cornerSW = (byte)cornerSW;
                tile.cornerSE = (byte)cornerSE;
                tile.cornerNW = (byte)cornerNW;
                tile.cornerNE = (byte)cornerNE;

                GetColorValue(tile, x, y);

                InternalSmooth(tile);
                world.LandTiles[tilePos.x, tilePos.y] = tile;

            }
            tilePos.y = 0;
        }

        TycoonMap.ScheduleOperation(new LoadTerrainOperation(world.LandTiles));
        TycoonMap.SchedulePaintOperation(new BeachLoadOperation(world.LandTiles));
    }

    void InternalSmooth(NetLandTile tile)
    {
        List<Corner> corners = new List<Corner>();

        corners.Add(new Corner("cornerNW", tile.cornerNW));
        corners.Add(new Corner("cornerNE", tile.cornerNE));
        corners.Add(new Corner("cornerSW", tile.cornerSW));
        corners.Add(new Corner("cornerSE", tile.cornerSE));

        corners.Sort((x, y) => x.height.CompareTo(y.height));

        if (corners[1].height != corners[2].height)
            return;

        corners[0].height = corners[1].height;

        foreach (Corner corner in corners)
        {
            switch (corner.name) 
            {
                case "cornerNW":
                    tile.cornerNW = corner.height;
                    return;
                case "cornerNE":
                    tile.cornerNE = corner.height;
                    return;
                case "cornerSW":
                    tile.cornerSW = corner.height;
                    return;
                case "cornerSE":
                    tile.cornerSE = corner.height;
                    return;
            }
        }
    } // maybe this this is not needed

    void GetColorValue(NetLandTile tile, int x, int y)
    {

        RGBA.Add("R", (int)BasicCoveridgeMap.GetPixel(x, y).r * 255);
        RGBA.Add("G", (int)BasicCoveridgeMap.GetPixel(x, y).g * 255);
        RGBA.Add("B", (int)BasicCoveridgeMap.GetPixel(x, y).b * 255);
        RGBA.Add("A", (int)BasicCoveridgeMap.GetPixel(x, y).a * 255);

        var rgbaList = RGBA.ToList();

        rgbaList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        RGBA.Clear();

        switch (rgbaList[3].Key)
        {
            case "R":
                tile.tileType = 0;
                break;
            case "G":
                tile.tileType = 3;
                return;
            case "B":
                tile.tileType = 2;
                return;
            case "A":
                //tile.tileType = 1;
                return;
        }

    }

}

public class Corner
{
    public string name;
    public byte height;
    public Corner(string _name, byte _value)
    {
        name = _name;
        height = _value;
    }
}


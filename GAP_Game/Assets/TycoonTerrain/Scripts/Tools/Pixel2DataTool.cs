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
    NetWorld world;
    int2 tilePos;


    IDictionary<string, byte> fourCorners = new Dictionary<string, byte>();

    bool top;
    bool left;
    bool right;
    bool bottom;
    bool topLeft;
    bool topRight;
    bool bottomLeft;
    bool bottomRight;



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

                float cornerSW = HeightMap.GetPixel(x + 1, y).a * 255f;
                float cornerSE = HeightMap.GetPixel(x, y).a * 255f;
                float cornerNW = HeightMap.GetPixel(x + 1, y + 1).a * 255f;
                float cornerNE = HeightMap.GetPixel(x, y + 1).a * 255f;

                NetLandTile tile = new NetLandTile();
                tile.cornerSW = (byte)cornerSW;
                tile.cornerSE = (byte)cornerSE;
                tile.cornerNW = (byte)cornerNW;
                tile.cornerNE = (byte)cornerNE;

                if (tilePos.y > 1020)
                    Debug.Log("yo");

                InternalSmooth(tile);
                world.LandTiles[tilePos.x, tilePos.y] = tile;

            }
            tilePos.y = 0;
        }

        ExternalSmooth(world);
        TycoonMap.ScheduleOperation(new LoadTerrainOperation(world.LandTiles));
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
    }

    void ExternalSmooth(NetWorld world)
    {
        for (int x = 0; x < world.LandTiles.Length; x++)
        {
            for (int y = 0; y < world.LandTiles.Length; y++)
            {
                if (!AvoidBorder(x, y))
                    continue;

                top = CheckTile(world.LandTiles[x, y +1]);
                left = CheckTile(world.LandTiles[x-1, y]);
                right = CheckTile(world.LandTiles[x+1, y]);
                bottom = CheckTile(world.LandTiles[x, y-1]);
                topLeft = CheckTile(world.LandTiles[x-1, y+1]);
                topRight = CheckTile(world.LandTiles[x+1, y+1]);
                bottomLeft = CheckTile(world.LandTiles[x-1, y-1]);
                bottomRight = CheckTile(world.LandTiles[x+1, y-1]);

                bool[] bools = { top, left, right, bottom, topLeft, topRight, bottomLeft, bottomRight };

                foreach(bool answer in bools)
                {
                    if (answer == false)
                        return;
                }

                byte flatValue = world.LandTiles[x + 1, y].cornerNE;

                world.LandTiles[x, y].cornerNE = flatValue;
                world.LandTiles[x, y].cornerSE = flatValue;
                world.LandTiles[x, y].cornerNW = flatValue;
                world.LandTiles[x, y].cornerSW = flatValue;


                ResetBools();
            }
        }

    }

    bool CheckTile(NetLandTile tile)
    {
        List<Corner> corners = new List<Corner>();

        corners.Add(new Corner("cornerNW", tile.cornerNW));
        corners.Add(new Corner("cornerNE", tile.cornerNE));
        corners.Add(new Corner("cornerSW", tile.cornerSW));
        corners.Add(new Corner("cornerSE", tile.cornerSE));

        corners.Sort((x, y) => x.height.CompareTo(y.height));

        if (corners[0].height != corners[1].height)
            return false;

        if (corners[1].height != corners[2].height)
            return false;

        return true;
    }

    bool AvoidBorder(int x, int y)
    {
        if (y < 1)
            return false;
        if (x < 1)
            return false;
        if (y > 1024)
            return false;
        if (x > 1024)
            return false;

        return true;
    }

    void ResetBools()
    {
        top = false;
        left = false;
        right = false;
        bottom = false;
        topLeft = false;
        topRight = false;
        bottomLeft = false;
        bottomRight = false;
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


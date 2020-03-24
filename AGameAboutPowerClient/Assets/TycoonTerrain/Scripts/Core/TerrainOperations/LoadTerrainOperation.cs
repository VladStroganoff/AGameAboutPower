using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;


namespace TycoonTerrain.Core.TerrainOperations
{
    public struct LoadTerrainOperation : ITerrainJob
    {


        public NetLandTile[,] heightData;
        public int2 dataSize;

        public LoadTerrainOperation(NetLandTile[,] heightMap)
        {
            heightData = heightMap; 
            dataSize = heightMap.GetLength(0);
        }

        public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks)
        {
            IntBound bounds = grid.IntersectBound(new IntBound(int2.zero, dataSize - new int2(1)));

            for (int x = bounds.Min.x; x <= bounds.Max.x; x++)
            {
                for (int z = bounds.Min.y; z <= bounds.Max.y; z++)
                {
                    int2 pos = new int2(x, z);
                    TileHandle tile = grid.GetTile(pos);
                    LandTile data = tile.GetData();

                    int4 heights = GetHeights(pos);
                    data.SetHeights(heights);

                    grid.SetTile(pos, data);
                }
            }

            dirtyChunks.MarkAllChunksDirty();
        }

        private int4 GetHeights(int2 position)
        {
            byte b0 = heightData[position.x, position.y].cornerNE;
            byte b1 = heightData[position.x, position.y].cornerSE;
            byte b2 = heightData[position.x, position.y].cornerSW;
            byte b3 = heightData[position.x, position.y].cornerNW;

            return new int4(b0, b1, b2, b3);
        }

    }

    public struct BeachLoadOperation : ITerrainPaintJob
    {
        private readonly NetLandTile[,] positions;

        public BeachLoadOperation(NetLandTile[,] positions)
        {
            this.positions = positions;
        }

        public void Execute(ref TerrainTypeTable terrainTypeTable, ref ChunkSet dirtyChunks)
        {
            for (int x = 0; x < positions.Length; x++)
            {
                for(int y = 0; y < positions.GetLength(0); y++)
                {
                    int2 pos = new int2(x,y);
                    terrainTypeTable.SetTerrainType(pos, positions[x, y].tileType);
                }
            }
        }
    }

}
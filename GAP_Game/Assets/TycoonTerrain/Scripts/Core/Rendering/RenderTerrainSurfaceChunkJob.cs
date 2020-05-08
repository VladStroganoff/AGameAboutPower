using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
    public struct RenderTerrainSurfaceChunkJob : IJob {
        private readonly IntBound chunk;
        [ReadOnly]
        private readonly TerrainGrid grid;
        [ReadOnly]
        private readonly TerrainTypeTable terrainTypeTable;

        private readonly float tileStepHeight;
        
        [ReadOnly]
        private NativeArray<float3> offsets;

        private NativeList<SubMeshTriangle> indices;
        private NativeList<float3> vertices;
        private NativeList<float2> uvs;
        
        public NativeMeshData NativeMeshData => new NativeMeshData(indices, vertices, uvs);

        public RenderTerrainSurfaceChunkJob(IntBound chunk, TerrainGrid grid, TerrainTypeTable terrainTypes, float tileStepHeight, NativeMeshData meshData) {
            this.chunk = chunk;
            this.grid = grid;
            this.tileStepHeight = tileStepHeight;
            terrainTypeTable = terrainTypes;
            indices = meshData.indices;
            vertices = meshData.vertices;
            uvs = meshData.uvs;
            offsets = new NativeArray<float3>(4, Allocator.TempJob);
            for (int i = 0; i < 4; i++) {
                offsets[i] = TileHandle.cornerOffsets[i];
            }
        }

        public void Dispose() {
            offsets.Dispose();
        }

        public void Execute() {
            for (int x = chunk.Min.x; x <= chunk.Max.x; x++) {
                for (int z = chunk.Min.y; z <= chunk.Max.y; z++) {
                    TileHandle handle = grid.GetTile(x, z);

                    ushort terrainType = terrainTypeTable.GetTerrainType(new int2(x, z));
                    GenerateTopFace(ref handle, terrainType);
                }
            }
        }

        private float3 GetCornerPosition(ref TileHandle handle, CornerIndex index) {
            float3 center = handle.CenterPosition;
            return offsets[index.Index] + center + new float3(0, handle.GetData().GetHeight(index) * tileStepHeight, 0);
        }

        private void GenerateTopFace(ref TileHandle handle, ushort terrainType) {
            int i = vertices.Length;

            float3 ne = GetCornerPosition(ref handle, CornerIndex.NorthEast);
            float3 se = GetCornerPosition(ref handle, CornerIndex.SouthEast);
            float3 sw = GetCornerPosition(ref handle, CornerIndex.SouthWest);
            float3 nw = GetCornerPosition(ref handle, CornerIndex.NorthWest);
            float3 center = handle.CenterPosition + new Vector3(0, handle.CenterSurfaceHeight * tileStepHeight, 0);
            float step = 1f / handle.Grid.Length;

            //North triangle
            vertices.Add(ne);
            vertices.Add(center);
            vertices.Add(nw);



            uvs.Add(new float2(ne.x* step, ne.z * step));
            uvs.Add(new float2(center.x * step, center.z * step));
            uvs.Add(new float2(nw.x * step, nw.z * step));


            //uvs.Add(new float2(1, 1));
            //uvs.Add(new float2(0.5f, 0.5f));
            //uvs.Add(new float2(0, 1));

            //East Triangle
            vertices.Add(ne);
            vertices.Add(se);
            vertices.Add(center);


            uvs.Add(new float2(ne.x * step, ne.z * step));
            uvs.Add(new float2(se.x * step, se.z * step));
            uvs.Add(new float2(center.x * step, center.z * step));

            //uvs.Add(new float2(1, 1));
            //uvs.Add(new float2(1, 0));
            //uvs.Add(new float2(0.5f, 0.5f));

            //South Triangle
            vertices.Add(se);
            vertices.Add(sw);
            vertices.Add(center);

            uvs.Add(new float2(se.x * step, se.z * step));
            uvs.Add(new float2(sw.x * step, sw.z * step));
            uvs.Add(new float2(center.x * step, center.z * step));

            //uvs.Add(new float2(1, 0));
            //uvs.Add(new float2(0, 0));
            //uvs.Add(new float2(0.5f, 0.5f));

            //West Triangle
            vertices.Add(nw);
            vertices.Add(center);
            vertices.Add(sw);


            uvs.Add(new float2(nw.x * step, nw.z * step));
            uvs.Add(new float2(center.x * step, center.z * step));
            uvs.Add(new float2(sw.x * step, sw.z * step));

            //uvs.Add(new float2(0, 1));
            //uvs.Add(new float2(0.5f, 0.5f));
            //uvs.Add(new float2(0, 0));

            indices.Add(new SubMeshTriangle(terrainType, i));
            indices.Add(new SubMeshTriangle(terrainType, i + 1));
            indices.Add(new SubMeshTriangle(terrainType, i + 2));

            indices.Add(new SubMeshTriangle(terrainType, i + 3));
            indices.Add(new SubMeshTriangle(terrainType, i + 4));
            indices.Add(new SubMeshTriangle(terrainType, i + 5));

            indices.Add(new SubMeshTriangle(terrainType, i + 6));
            indices.Add(new SubMeshTriangle(terrainType, i + 7));
            indices.Add(new SubMeshTriangle(terrainType, i + 8));

            indices.Add(new SubMeshTriangle(terrainType, i + 9));
            indices.Add(new SubMeshTriangle(terrainType, i + 10));
            indices.Add(new SubMeshTriangle(terrainType, i + 11));
        }
    }
}
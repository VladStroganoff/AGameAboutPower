using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
    public struct RenderTerrainCliffChunkJob : IJob {
        private readonly IntBound chunk;

        [ReadOnly]
        private readonly TerrainGrid grid;

        [ReadOnly] 
        private readonly TerrainTypeTable terrainTypeTable;

        private readonly float3 tileStepHeight;

        private NativeList<SubMeshTriangle> indices;
        private NativeList<float3> vertices;
        private NativeList<float2> uvs;

        public RenderTerrainCliffChunkJob(IntBound chunk, TerrainGrid grid, TerrainTypeTable typeTable, float tileStepHeight, NativeMeshData nativeMeshData) {
            this.chunk = chunk;
            this.grid = grid;
            this.tileStepHeight = new float3(1, tileStepHeight, 1);
            terrainTypeTable = typeTable;
            indices = nativeMeshData.indices;
            vertices = nativeMeshData.vertices;
            uvs = nativeMeshData.uvs;
        }

        public NativeMeshData NativeMeshData => new NativeMeshData(indices, vertices, uvs);

        public void Execute() {
            for (int x = chunk.Min.x; x <= chunk.Max.x; x++) {
                for (int z = chunk.Min.y; z <= chunk.Max.y; z++) {
                    TileHandle handle = grid.GetTile(x, z);

                    GenerateCliffsFor(handle);
                }
            }
        }

        private void GenerateCliffsFor(TileHandle handle) {
            GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.North), CornerIndex.NorthWest);
            GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.East), CornerIndex.NorthEast);
            GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.South), CornerIndex.SouthEast);
            GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.West), CornerIndex.SouthWest);
        }

        /// <summary>
        /// Generates a quad or a triangle for a single cliff direction.
        /// </summary>
        /// <param name="handle">The tile handle for the origin tile of the cliff</param>
        /// <param name="neighbour">The tile handle of the neighbouring tile that this cliff is facing</param>
        /// <param name="startIndex">The left cornerindex of the origin tile when pointed towards the neighbour tile</param>
        private void GenerateCliff(TileHandle handle, TileHandle neighbour, CornerIndex startIndex) {
            //Directions are relative when looking from the origin tile to the neighbour tile
            CornerIndex neighbourLeft = startIndex.NeighbourCounterClockwise;
            CornerIndex neighbourRight = startIndex.NeighbourOpposite;

            bool needsTriangleA = !neighbour.IsInBounds || neighbour.GetHeight(neighbourLeft) < handle.GetHeight(startIndex);
            bool needsTriangleB = !neighbour.IsInBounds || neighbour.GetHeight(neighbourRight) < handle.GetHeight(startIndex.NeighbourClockwise);

            //Either both tiles' edges align, or the neighbouring tile edge is higher, so we don't need to generate a cliff for the origin tile
            if (!needsTriangleA && !needsTriangleB) {
                return;
            }

            //We will always need the two vertices at the origin tiles' top edge.
            int i = vertices.Length;

            ushort terrainType = terrainTypeTable.GetTerrainType(handle.tilePosition);
            float step = 1f / handle.Grid.Length;

            vertices.Add(tileStepHeight * handle.GetCornerPosition(startIndex.NeighbourClockwise));
            vertices.Add(tileStepHeight * handle.GetCornerPosition(startIndex));

            uvs.Add(new float2(handle.GetCornerPosition(startIndex.NeighbourClockwise).x*step, handle.GetCornerPosition(startIndex.NeighbourClockwise).z * step));
            uvs.Add(new float2(handle.GetCornerPosition(startIndex).x*step, handle.GetCornerPosition(startIndex).z * step));

            //uvs.Add(new float2(0, 0));
            //uvs.Add(new float2(1, 0));


            //If both triangles are required
            if (needsTriangleA && needsTriangleB) {

                vertices.Add(tileStepHeight * neighbour.GetCornerPosition(neighbourLeft));
                vertices.Add(tileStepHeight * neighbour.GetCornerPosition(neighbourRight));


                uvs.Add(new float2(neighbour.GetCornerPosition(neighbourLeft).x*step, neighbour.GetCornerPosition(neighbourLeft).z * step));
                uvs.Add(new float2(neighbour.GetCornerPosition(neighbourRight).x*step, neighbour.GetCornerPosition(neighbourRight).z * step));
                //uvs.Add(new float2(1, 1));
                //uvs.Add(new float2(0, 1));

                indices.Add(new SubMeshTriangle(terrainType, i));
                indices.Add(new SubMeshTriangle(terrainType, i + 1));
                indices.Add(new SubMeshTriangle(terrainType, i + 3));

                indices.Add(new SubMeshTriangle(terrainType, i + 1));
                indices.Add(new SubMeshTriangle(terrainType, i + 2));
                indices.Add(new SubMeshTriangle(terrainType, i + 3));
            }
            else {
                if (needsTriangleA) {
                    vertices.Add(tileStepHeight * neighbour.GetCornerPosition(neighbourLeft));
                    uvs.Add(new float2(neighbour.GetCornerPosition(neighbourLeft).x*step, neighbour.GetCornerPosition(neighbourLeft).z * step));
                    //uvs.Add(new float2(1, 1));
                }
                else {
                    vertices.Add(tileStepHeight * neighbour.GetCornerPosition(neighbourRight));
                    uvs.Add(new float2(neighbour.GetCornerPosition(neighbourRight).x*step, neighbour.GetCornerPosition(neighbourRight).z * step));
                    //uvs.Add(new float2(0, 1));
                }

                indices.Add(new SubMeshTriangle(terrainType, i));
                indices.Add(new SubMeshTriangle(terrainType, i + 1));
                indices.Add(new SubMeshTriangle(terrainType, i + 2));
            }
        }
    }
}
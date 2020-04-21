using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	/// <summary>
	/// This job renders the water surface of a chunk.
	/// </summary>
	public struct RenderWaterChunkJob : IJob {
		private readonly IntBound chunk;

		[ReadOnly]
		private readonly TerrainGrid grid;

		//Native lists for mesh data.
		private NativeList<SubMeshTriangle> indices;
		private NativeList<float3> vertices;
		private NativeList<float2> uvs;
		
		private readonly float waterStepHeight;

		/// <summary>
		/// Render the water surface of a given chunk.
		/// </summary>
		/// <param name="chunk">The chunk of <paramref name="grid"/> to generate water surface for.</param>
		/// <param name="grid">The terrain grid.</param>
		/// <param name="nativeMeshData">The native mesh data.</param>
		/// <param name="waterStepHeight">The water step height.</param>
		public RenderWaterChunkJob(IntBound chunk, TerrainGrid grid, NativeMeshData nativeMeshData, float waterStepHeight) {
			this.chunk = chunk;
			this.grid = grid;
			this.indices = nativeMeshData.indices;
			vertices = nativeMeshData.vertices;
			uvs = nativeMeshData.uvs;
			this.waterStepHeight = waterStepHeight;
		}

		public NativeMeshData NativeMeshData => new NativeMeshData(indices, vertices, uvs);

		public void Execute() {
			for (int x = chunk.Min.x; x <= chunk.Max.x; x++) {
				for (int z = chunk.Min.y; z <= chunk.Max.y; z++) {
					TileHandle handle = grid.GetTile(x, z);

					GenerateWaterSurface(handle);
				}
			}
		}

		private void GenerateWaterSurface(TileHandle handle) {
			//Don't draw the water surface if it is completely below the terrain
			if (handle.IsWaterLevelBelowSurface) {
				return;
			}

			int i = vertices.Length;

			Vector3 waterOffset = handle.CenterPosition + new Vector3(0, waterStepHeight * handle.GetData().WaterLevel, 0);

			vertices.Add(waterOffset + handle.GetBaseCornerOffset(CornerIndex.NorthEast));
			vertices.Add(waterOffset + handle.GetBaseCornerOffset(CornerIndex.SouthEast));
			vertices.Add(waterOffset + handle.GetBaseCornerOffset(CornerIndex.SouthWest));
			vertices.Add(waterOffset + handle.GetBaseCornerOffset(CornerIndex.NorthWest));

			uvs.Add(new float2(1, 1));
			uvs.Add(new float2(1, 0));
			uvs.Add(new float2(0, 0));
			uvs.Add(new float2(0, 1));

			const ushort terrainType = 0;

			indices.Add(new SubMeshTriangle(terrainType, i));
			indices.Add(new SubMeshTriangle(terrainType, i + 1));
			indices.Add(new SubMeshTriangle(terrainType, i + 3));

			indices.Add(new SubMeshTriangle(terrainType, i + 3));
			indices.Add(new SubMeshTriangle(terrainType, i + 1));
			indices.Add(new SubMeshTriangle(terrainType, i + 2));
		}
	}
}
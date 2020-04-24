using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	/// <summary>
	/// This job is responsible for rendering the water cliffs, at the map edges, to a mesh.
	/// </summary>
	public struct RenderWaterCliffChunkJob : IJob {
		private const ushort terrainType = 0;
		private const float gridSize = 1f;

		private readonly IntBound bounds;
		[ReadOnly]
		private readonly TerrainGrid grid;

		private readonly float tileStepHeight;
		private readonly float waterHeightStep;
		private readonly int waterStepsPerHeight;
		
		private NativeList<SubMeshTriangle> indices;
		private NativeList<float3> vertices;
		private NativeList<float2> uvs;
		
		public NativeMeshData NativeMeshData => new NativeMeshData(indices, vertices, uvs);

		public RenderWaterCliffChunkJob(IntBound bounds, TerrainGrid grid, NativeMeshData nativeMeshData, float tileStepHeight, float waterHeightStep) {
			this.bounds = bounds;
			this.grid = grid;
			this.waterHeightStep = waterHeightStep;
			this.tileStepHeight = tileStepHeight;
			waterStepsPerHeight = grid.WaterHeightStepsPerTileHeight;
			indices = nativeMeshData.indices;
			vertices = nativeMeshData.vertices;
			uvs = nativeMeshData.uvs;
		}

		//Loop over every tile within the bounds
		public void Execute() {
			for (int x = bounds.Min.x; x <= bounds.Max.x; x++) {
				for (int z = bounds.Min.y; z <= bounds.Max.y; z++) {
					TileHandle handle = grid.GetTile(x, z);

					GenerateWaterCliff(handle);
				}
			}
		}

		//Generate cliffs for all 4 sides. For each side, start with the corner on the right when facing outwards.
		private void GenerateWaterCliff(TileHandle handle) {
			GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.North), CornerIndex.NorthWest);
			GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.East), CornerIndex.NorthEast);
			GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.South), CornerIndex.SouthEast);
			GenerateCliff(handle, handle.GetNeighbourOrDefault(CardinalDirection.West), CornerIndex.SouthWest);
		}

		private void GenerateCliff(TileHandle handle, TileHandle neighbour, CornerIndex startIndex) {
			int waterLevelInt = handle.GetData().WaterLevel;
			float waterLevel = waterHeightStep * waterLevelInt;

			//Directions are relative when looking from the origin tile to the neighbour tile
			CornerIndex neighbourLeft = startIndex.NeighbourCounterClockwise;
			CornerIndex neighbourRight = startIndex.NeighbourOpposite;

			CornerIndex ownLeft = startIndex;
			CornerIndex ownRight = startIndex.NeighbourClockwise;

			float3 tileHeightMultiplier = new float3(1, tileStepHeight, 1);

			bool leftUnderWater = handle.GetHeight(ownLeft) * waterStepsPerHeight < waterLevelInt && (!neighbour.IsInBounds ||  handle.GetData().WaterLevel != neighbour.GetData().WaterLevel && waterLevelInt > neighbour.GetHeight(neighbourRight) * waterStepsPerHeight);
			bool rightUnderWater = handle.GetHeight(ownRight) * waterStepsPerHeight < waterLevelInt && (!neighbour.IsInBounds || handle.GetData().WaterLevel != neighbour.GetData().WaterLevel && waterLevelInt > neighbour.GetHeight(neighbourLeft) * waterStepsPerHeight);

			//Either both tiles' edges align, or the neighbouring tile edge is higher, so we don't need to generate a cliff for the origin tile
			if (!leftUnderWater && !rightUnderWater) {
				return;
			}

			//We will always need the two vertices at the origin tiles' top edge.
			int i = vertices.Length;

			//If both triangles are required
			if (leftUnderWater && rightUnderWater) {

				float3 topRightPosition = handle.GetCornerPosition(ownRight) * new float3(1, 0, 1) + new float3(0, waterLevel, 0);
				float3 topLeftPosition = handle.GetCornerPosition(ownLeft) * new float3(1, 0, 1) + new float3(0, waterLevel, 0);

				vertices.Add(topRightPosition);
				vertices.Add(topLeftPosition);

				vertices.Add(tileHeightMultiplier * handle.GetCornerPosition(ownLeft));
				vertices.Add(tileHeightMultiplier * handle.GetCornerPosition(ownRight));

				uvs.Add(new float2(0, 0));
				uvs.Add(new float2(1, 0));
				uvs.Add(new float2(1, 1));
				uvs.Add(new float2(0, 1));

				indices.Add(new SubMeshTriangle(terrainType, i));
				indices.Add(new SubMeshTriangle(terrainType, i + 1));
				indices.Add(new SubMeshTriangle(terrainType, i + 3));

				indices.Add(new SubMeshTriangle(terrainType, i + 1));
				indices.Add(new SubMeshTriangle(terrainType, i + 2));
				indices.Add(new SubMeshTriangle(terrainType, i + 3));
			}
			else {

				float3 terrainRightPos = tileHeightMultiplier * handle.GetCornerPosition(ownRight);
				float3 terrainLeftPos = tileHeightMultiplier * handle.GetCornerPosition(ownLeft);

				float deltaWaterHeight = waterLevel - math.min(terrainRightPos.y, terrainLeftPos.y);
				float xEdge = (gridSize * deltaWaterHeight) / tileStepHeight;

				//Select whether the left or right corner is the lower or higher corner
				float3 lowPosition = terrainLeftPos.y <= terrainRightPos.y ? terrainLeftPos : terrainRightPos;
				float3 highPosition = terrainLeftPos.y > terrainRightPos.y ? terrainLeftPos : terrainRightPos;
				float3 midPosition = math.lerp(lowPosition, highPosition, math.clamp(xEdge, 0f, 1f));

				float3 lowTopPosition = lowPosition * new float3(1, 0, 1) + new float3(0, waterLevel, 0);

				if (leftUnderWater) {
					vertices.Add(lowPosition);
					vertices.Add(midPosition);
					vertices.Add(lowTopPosition);

					uvs.Add(new float2(1, 0));
					uvs.Add(new float2(0, 1));
					uvs.Add(new float2(1, 1));
				}
				else {
					vertices.Add(lowTopPosition);
					vertices.Add(midPosition);
					vertices.Add(lowPosition);

					uvs.Add(new float2(0, 1));
					uvs.Add(new float2(1, 1));
					uvs.Add(new float2(0, 0));
				}

				indices.Add(new SubMeshTriangle(terrainType, i));
				indices.Add(new SubMeshTriangle(terrainType, i + 1));
				indices.Add(new SubMeshTriangle(terrainType, i + 2));
			}
		}
	}
}
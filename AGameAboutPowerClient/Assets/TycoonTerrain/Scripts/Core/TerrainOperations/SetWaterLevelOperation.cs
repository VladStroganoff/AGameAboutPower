using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct SetWaterLevelOperation : ITerrainJob {
		private readonly NativeArray<int2> tilePositions;
		private readonly ushort targetWaterLevel;

		public SetWaterLevelOperation(NativeArray<int2> tiles, ushort newWaterLevel) {
			tilePositions = tiles;
			targetWaterLevel = newWaterLevel;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			int WaterHeightStepsPerTileHeight = grid.WaterHeightStepsPerTileHeight;

			for (int i = 0; i < tilePositions.Length; i++) {
				int2 pos = tilePositions[i];
				LandTile tile = grid.GetTileData(pos);

				//Check if the target water level is completely below surface and if so, set the water level to 0
				if (targetWaterLevel <= tile.GetLowestPoint() * WaterHeightStepsPerTileHeight) {
					tile.ClearWater();
				}
				else {
					tile.SetWaterLevel(targetWaterLevel);
				}

				grid.SetTile(pos, tile);
				dirtyChunks.MarkChunkDirtyAtWorldPosition(pos);
			}
		}
	}
}
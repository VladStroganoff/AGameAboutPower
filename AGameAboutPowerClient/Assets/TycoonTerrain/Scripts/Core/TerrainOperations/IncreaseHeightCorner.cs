using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct IncreaseHeightCorner : ITerrainJob {
		private readonly int2 position;
		private readonly CornerIndex corner;

		public IncreaseHeightCorner(int2 tilePosition, CornerIndex cornerIndex) {
			position = tilePosition;
			corner = cornerIndex;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			TileHandle handle = grid.GetTile(position);
			grid.IncreaseHeight(handle, corner);

			dirtyChunks.MarkChunkDirtyAtWorldPosition(position);
		}
	}
}
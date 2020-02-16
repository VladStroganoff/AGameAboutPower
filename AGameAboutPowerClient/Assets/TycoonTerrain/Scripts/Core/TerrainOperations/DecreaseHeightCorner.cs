using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct DecreaseHeightCorner : ITerrainJob {
		private readonly int2 position;
		private readonly CornerIndex corner;

		public DecreaseHeightCorner(int2 tilePosition, CornerIndex cornerIndex) {
			position = tilePosition;
			corner = cornerIndex;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			TileHandle handle = grid.GetTile(position);
			grid.DecreaseHeight(handle, corner);

			dirtyChunks.MarkChunkDirtyAtWorldPosition(position);
		}
	}
}
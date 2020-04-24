namespace TycoonTerrain.Core.TerrainOperations {
	/// <summary>
	/// This operation will increase the height of the tiles within the specified bounds, starting from the lowest corners within that bound.
	/// Cliffs will be generated at the bounds edges.
	/// </summary>
	public struct IncreaseHeightCliffOperation : ITerrainJob {
		private readonly IntBound selectionBounds;

		public IncreaseHeightCliffOperation(IntBound bounds) {
			selectionBounds = bounds;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			var bounds = grid.Bounds.Intersection(selectionBounds);

			int lowest = grid.GetLowestInBounds(bounds);

			bool allEqual = grid.IsFlatInBounds(bounds);

			foreach (TileHandle tileHandle in grid.GetTilesIn(bounds)) {
				if (tileHandle.GetData().GetLowestPoint() <= lowest || allEqual) {
					grid.IncreaseHeight(tileHandle);
				}
			}

			dirtyChunks.MarkRegionDirty(bounds);
		}
	}
}
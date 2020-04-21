namespace TycoonTerrain.Core.TerrainOperations {
	/// <summary>
	/// This operation will decrease the height of the tiles within the specified bounds, starting from the highest corners within that bound.
	/// Cliffs will be generated at the bounds edges.
	/// </summary>
	public struct DecreaseHeightCliffOperation : ITerrainJob {
		private readonly IntBound selectionBounds;

		public DecreaseHeightCliffOperation(IntBound bounds) {
			selectionBounds = bounds;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			var bounds = grid.Bounds.Intersection(selectionBounds);

			int highest = grid.GetHighestInBounds(bounds);

			//If all heights are equal, all height are at the highest height and thus all should be lowered.
			bool allEqual = grid.IsFlatInBounds(bounds);

			foreach (TileHandle tileHandle in grid.GetTilesIn(bounds)) {
				if (tileHandle.GetData().GetHighestPoint() >= highest || allEqual) {
					grid.DecreaseHeight(tileHandle);
				}
			}

			dirtyChunks.MarkRegionDirty(bounds);
		}
	}
}
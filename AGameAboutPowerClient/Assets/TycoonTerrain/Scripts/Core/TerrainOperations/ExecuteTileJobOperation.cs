using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct ExecuteTileJobOperation<T> : ITerrainJob where T : struct, ITileJob {

		private readonly T operation;
		private readonly IntBound operationBounds;

		public ExecuteTileJobOperation(T operation, IntBound bounds) {
			this.operation = operation;
			this.operationBounds = bounds;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			IntBound bounds = grid.IntersectBound(operationBounds);

			for (int x = bounds.Min.x; x <= bounds.Max.x; x++) {
				for (int z = bounds.Min.y; z <= bounds.Max.y; z++) {

					int2 position = new int2(x, z);
					LandTile tile = grid.GetTile(position).GetData();

					operation.Execute(ref tile, position);

					//Apply the changes made in operation back to the grid
					grid.SetTile(position, tile);
				}
			}

			dirtyChunks.MarkRegionDirty(bounds);
		}
	}
}
namespace TycoonTerrain.Core.PaintOperations {
	public struct PaintTerrainBoundsOperation : ITerrainPaintJob {
		private readonly ushort terrainType;
		private readonly IntBound bounds;

		public PaintTerrainBoundsOperation(IntBound bounds, ushort terrainType) {
			this.bounds = bounds;
			this.terrainType = terrainType;
		}

		public void Execute(ref TerrainTypeTable terrainTypeTable, ref ChunkSet dirtyChunks) {
			terrainTypeTable.SetTerrainType(bounds, terrainType);

			dirtyChunks.MarkRegionDirty(bounds);
		}
	}
}
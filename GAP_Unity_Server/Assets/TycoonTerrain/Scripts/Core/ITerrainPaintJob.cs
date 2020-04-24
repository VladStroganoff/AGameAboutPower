namespace TycoonTerrain.Core {
	public interface ITerrainPaintJob {
		void Execute(ref TerrainTypeTable terrainTypeTable, ref ChunkSet dirtyChunks);
	}
}
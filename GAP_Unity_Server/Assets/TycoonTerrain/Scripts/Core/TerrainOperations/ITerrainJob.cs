namespace TycoonTerrain.Core.TerrainOperations {
	public interface ITerrainJob {
		void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks);
	}
}

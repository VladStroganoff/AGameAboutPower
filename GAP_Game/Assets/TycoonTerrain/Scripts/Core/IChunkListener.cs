namespace TycoonTerrain.Core {
	public interface IChunkListener {
		void OnUpdateChunks(TycoonTileMap terrain, ref ChunkSet chunks);
	}
}
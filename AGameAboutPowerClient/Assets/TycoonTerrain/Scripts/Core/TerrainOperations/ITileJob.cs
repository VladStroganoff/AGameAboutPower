using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public interface ITileJob {
		void Execute(ref LandTile tile, int2 position);
	}
}
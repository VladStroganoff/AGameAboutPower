using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct SetHeightOperation : ITileJob {
		private readonly int height;

		public SetHeightOperation(int height) {
			this.height = height;
		}

		public void Execute(ref LandTile tile, int2 position) {
			tile.SetHeights(new int4(height));
		}
	}
}
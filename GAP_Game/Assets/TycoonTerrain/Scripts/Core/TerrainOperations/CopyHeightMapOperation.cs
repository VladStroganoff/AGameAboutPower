using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct CopyHeightMapOperation : ITerrainJob {
		public NativeArray<byte> heightData;
		public int2 dataSize;

		public CopyHeightMapOperation(NativeArray<byte> heightMap, int2 dimensions) {
			heightData = heightMap;
			dataSize = dimensions;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			IntBound bounds = grid.IntersectBound(new IntBound(int2.zero, dataSize - new int2(1)));

			for (int x = bounds.Min.x; x <= bounds.Max.x; x++) {
				for (int z = bounds.Min.y; z <= bounds.Max.y; z++) {
					int2 pos = new int2(x, z);
					TileHandle tile = grid.GetTile(pos);
					LandTile data = tile.GetData();

					int4 heights = GetHeights(pos);
					data.SetHeights(heights);

					grid.SetTile(pos, data);
				}
			}

			dirtyChunks.MarkAllChunksDirty();
		}

		private int4 GetHeights(int2 position) {
			int2 p0 = position + new int2(1, 1);
			int2 p1 = position + new int2(1, 0);
			int2 p2 = position + new int2(0, 0);
			int2 p3 = position + new int2(0, 1);

			byte b0 = heightData[p0.x + dataSize.x * p0.y];
			byte b1 = heightData[p1.x + dataSize.x * p1.y];
			byte b2 = heightData[p2.x + dataSize.x * p2.y];
			byte b3 = heightData[p3.x + dataSize.x * p3.y];

			return new int4(b0, b1, b2, b3);
		}
	}
}
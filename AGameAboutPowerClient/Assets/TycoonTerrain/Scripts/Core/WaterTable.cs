using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	public struct WaterTable {
		private readonly int2 size;
		private readonly Allocator allocator;
		private readonly NativeArray<ushort> table;

		public WaterTable(int width, int length, Allocator allocator) {
			size = new int2(width, length);
			this.allocator = allocator;
			table = new NativeArray<ushort>(width * length, allocator);
		}

		public int GetWaterLevel(int2 pos) {
			return table[pos.x + pos.y * size.y];
		}
	}
}
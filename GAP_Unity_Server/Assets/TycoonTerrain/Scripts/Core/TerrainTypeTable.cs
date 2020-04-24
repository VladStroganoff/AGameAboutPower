using System;
using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	public struct TerrainTypeTable {
		private int2 size;
		private NativeArray<ushort> grid;

		public int2 Size => size;

		private IntBound gridBounds => new IntBound(int2.zero, size - new int2(1));

		public TerrainTypeTable(int2 size, Allocator allocator) {
			this.size = size;
			grid = new NativeArray<ushort>(size.x * size.y, allocator);
		}

		public ushort GetTerrainType(int2 position) {
			return grid[GetIndex(position)];
		}

		public void SetTerrainType(int2 position, ushort value) {
			grid[GetIndex(position)] = value;
		}

		public void SetTerrainType(IntBound bounds, ushort value) {
			bounds = gridBounds.Intersection(bounds);

			for (int x = bounds.Min.x; x <= bounds.Max.x; x++) {
				for (int z = bounds.Min.y; z <= bounds.Max.y; z++) {
					SetTerrainType(new int2(x, z), value);
				}
			}
		}

		private int GetIndex(int2 position) {
			return position.x + position.y * size.x;
		}

		internal void Dispose() {
			grid.Dispose();
		}
	}
}
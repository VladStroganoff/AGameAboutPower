﻿using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct SetHeightSmoothOperation : ITerrainJob {
		public IntBound selection;
		public int height;

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			IntBound correctedBound = new IntBound(selection.Min, selection.Max + new int2(1));
			IntBound bounds = grid.IntersectBound(correctedBound.Expand(new int2(height)));
			height = math.clamp(height, 0, byte.MaxValue);

			for (int x = bounds.Min.x; x < bounds.Max.x; x++) {
				for (int z = bounds.Min.y; z < bounds.Max.y; z++) {
					int2 pos = new int2(x, z);

					int4 heights = new int4(0);
					foreach (CornerIndex cornerIndex in CornerIndex.All) {
						int2 correctedpos = pos + cornerIndex.ToVertexOffset();
						int manhattenDistance = correctedBound.Distance(correctedpos);
						heights[cornerIndex.Index] = math.clamp(heights[cornerIndex.Index], height - manhattenDistance, height + manhattenDistance);
					}

					LandTile tile = grid.GetTile(pos).GetData();
					tile.MaxHeight(heights);
					grid.SetTile(pos, tile);
				}
			}

			dirtyChunks.MarkRegionDirty(bounds);
		}
	}
}
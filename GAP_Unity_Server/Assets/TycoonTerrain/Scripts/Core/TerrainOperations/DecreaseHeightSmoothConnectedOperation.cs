using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct DecreaseHeightSmoothConnectedOperation : ITerrainJob {

		private readonly IntBound operationBounds;

		public DecreaseHeightSmoothConnectedOperation(IntBound bounds) {
			operationBounds = bounds;
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			NativeArray<int2> directions = new NativeArray<int2>(4, Allocator.Temp);
			directions[0] = new int2(0, 1);
			directions[1] = new int2(1, 0);
			directions[2] = new int2(0, -1);
			directions[3] = new int2(-1, 0);

			//TODO: Remove this managed allocation such that this can be used in a Job
			Stack<HeightChangeData> queue = new Stack<HeightChangeData>();

			byte maxHeight = grid.GetHighestInBounds(operationBounds);

			foreach (TileHandle handle in grid.GetTilesIn(operationBounds)) {
				if(handle.GetHeight(CornerIndex.NorthEast) >= maxHeight) queue.Push(new HeightChangeData(handle, CornerIndex.NorthEast));
				if(handle.GetHeight(CornerIndex.SouthEast) >= maxHeight) queue.Push(new HeightChangeData(handle, CornerIndex.SouthEast));
				if(handle.GetHeight(CornerIndex.SouthWest) >= maxHeight) queue.Push(new HeightChangeData(handle, CornerIndex.SouthWest));
				if(handle.GetHeight(CornerIndex.NorthWest) >= maxHeight) queue.Push(new HeightChangeData(handle, CornerIndex.NorthWest));
			}
			
			while (queue.Count > 0) {
				HeightChangeData data = queue.Pop();

				if (data.landPatch.GetHeight(data.cornerIndex) == data.cornerHeight) {
					LandTile oldTile = data.landPatch.GetData();
					int cost = grid.DecreaseHeight(data.landPatch, data.cornerIndex);

					if (cost != 0) {
						dirtyChunks.MarkChunkDirtyAtWorldPosition(data.landPatch.tilePosition);

						CornerIndex index = data.cornerIndex;
						for (int i = 0; i < 4; i++) {
							if (data.landPatch.GetHeight(index) != oldTile.GetHeight(index)) {

								//Get the tile on the left hand side of this corner
								int2 lhsPos = data.landPatch.tilePosition + directions[index.Index];
								if (grid.IsInBounds(lhsPos)) {
									TileHandle lhs = grid.GetTile(lhsPos);
									CornerIndex lhsIndex = index.NeighbourClockwise;
									//Check if in the old situation, the two neighboring corners were flush
									if (oldTile.GetHeight(index) - lhs.GetHeight(lhsIndex) == 0) {
										queue.Push(new HeightChangeData(lhs, lhsIndex));
									}
								}

								//Get the tile on the right hand side of this corner
								int2 rhsPos = data.landPatch.tilePosition + directions[index.NeighbourClockwise.Index];
								if (grid.IsInBounds(rhsPos)) {
									TileHandle rhs = grid.GetTile(rhsPos);
									CornerIndex rhsIndex = index.NeighbourCounterClockwise;
									//Check if in the old situation, the two neighboring corners were flush
									if (oldTile.GetHeight(index) - rhs.GetHeight(rhsIndex) == 0) {
										queue.Push(new HeightChangeData(rhs, rhsIndex));
									}
								}
							}
							index = index.NeighbourClockwise;
						}
					}
				}
			}
		}

		private struct HeightChangeData {
			public HeightChangeData(TileHandle landPatch, CornerIndex cornerIndex, byte cornerHeight) {
				this.landPatch = landPatch;
				this.cornerIndex = cornerIndex;
				this.cornerHeight = cornerHeight;
			}

			public HeightChangeData(TileHandle landpatch, CornerIndex cornerIndex) {
				landPatch = landpatch;
				this.cornerIndex = cornerIndex;
				cornerHeight = landpatch.GetHeight(cornerIndex);
			}

			public TileHandle landPatch;

			public CornerIndex cornerIndex;

			public byte cornerHeight;
		}
	}
}
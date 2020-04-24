using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	public struct GetTilesInWaterBody : ITerrainJob {
		public NativeList<int2> Results { get; private set; }
		private readonly int2 rootPosition;

		private readonly HashSet<int2> visited;
		private readonly Queue<int2> unexplored;

		public GetTilesInWaterBody(int2 position, NativeList<int2> results) {
			rootPosition = position;
			Results = results;

			visited = new HashSet<int2>();
			unexplored = new Queue<int2>();
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			TileHandle rootTile = grid.GetTile(rootPosition);

			//Check if the water level is completely below the terrain height of this tile
			if (rootTile.IsWaterLevelBelowSurface) {
				return;
			}

			int2 northDir = CardinalDirection.North.ToVector();
			int2 eastDir = CardinalDirection.East.ToVector();
			int2 southDir = CardinalDirection.South.ToVector();
			int2 westDir = CardinalDirection.West.ToVector();

			unexplored.Enqueue(rootPosition);

			while (unexplored.Count > 0) {
				int2 pos = unexplored.Dequeue();

				if(visited.Contains(pos))
					continue;

				TileHandle handle = grid.GetTile(pos);

				if (handle.GetData().WaterLevel != rootTile.GetData().WaterLevel) {
					continue;
				}

				Results.Add(pos);
				visited.Add(handle.tilePosition);

				//Enqueue all neighbours
				EnqueueNeighbour(grid, pos + northDir);
				EnqueueNeighbour(grid, pos + eastDir);
				EnqueueNeighbour(grid, pos + southDir);
				EnqueueNeighbour(grid, pos + westDir);
			}
		}

		/// <summary>
		/// Enqueues a new position to the unexplored queue.
		/// </summary>
		/// <param name="grid">The terrain grid.</param>
		/// <param name="position">The position to enqueue.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnqueueNeighbour(TerrainGrid grid, int2 position) {
			if(grid.IsInBounds(position) && !visited.Contains(position))
				unexplored.Enqueue(position);
		}
	}
}
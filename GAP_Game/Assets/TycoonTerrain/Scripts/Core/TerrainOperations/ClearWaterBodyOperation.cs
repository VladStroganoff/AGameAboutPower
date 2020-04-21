using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	/// <summary>
	/// This operation is responsible for removing all water that is connected to the water in the origin tile.
	/// </summary>
	public struct ClearWaterBodyOperation : ITerrainJob {
		private readonly int2 origin;
		private readonly ushort originWaterLevel;

		private readonly HashSet<int2> visited;
		private readonly Queue<int2> unexplored;

		public ClearWaterBodyOperation(TileHandle tile) {
			origin = tile.tilePosition;
			originWaterLevel = tile.GetData().WaterLevel;

			visited = new HashSet<int2>();
			unexplored = new Queue<int2>(400);
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {
			int2 northDir = CardinalDirection.North.ToVector();
			int2 eastDir = CardinalDirection.East.ToVector();
			int2 southDir = CardinalDirection.South.ToVector();
			int2 westDir = CardinalDirection.West.ToVector();

			unexplored.Enqueue(origin);

			while (unexplored.Count > 0) {
				int2 pos = unexplored.Dequeue();
				TileHandle handle = grid.GetTile(pos);

				//Only process if the current tile has water connected to the origin tile
				if (handle.GetData().WaterLevel != originWaterLevel) {
					continue;
				}

				dirtyChunks.MarkChunkDirtyAtWorldPosition(pos);
				visited.Add(pos);
				var tile = handle.GetData();
				tile.ClearWater();
				grid.SetTile(pos, tile);

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
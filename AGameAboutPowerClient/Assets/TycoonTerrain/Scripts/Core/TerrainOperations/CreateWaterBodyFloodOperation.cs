using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace TycoonTerrain.Core.TerrainOperations {
	/// <summary>
	/// This operation will create a water body using a floodfill, starting from a given position and water level.
	/// Neighbouring tiles will also be filled with water as long as any of the neighbour height are below water level.
	/// </summary>
	public struct CreateWaterBodyFloodOperation : ITerrainJob {
		private readonly int newWaterLevel;
		private readonly int WaterHeightStepsPerTileHeight;
		private readonly int2 startPosition;

		private readonly Queue<int2> unexplored;
		private readonly HashSet<int2> visited;

		public CreateWaterBodyFloodOperation(int2 position, int waterLevel, int waterStepsPerHeight) {
			newWaterLevel = math.clamp(waterLevel, 0, ushort.MaxValue);
			startPosition = position;
			WaterHeightStepsPerTileHeight = waterStepsPerHeight;

			visited = new HashSet<int2>();
			unexplored = new Queue<int2>(400);
		}

		public void Execute(ref TerrainGrid grid, ref ChunkSet dirtyChunks) {

			ushort height = (ushort)newWaterLevel;

			byte maxHeight = (byte)math.clamp((int)math.floor(newWaterLevel / (float)WaterHeightStepsPerTileHeight), 0, byte.MaxValue);

			int2 northDir = CardinalDirection.North.ToVector();
			int2 eastDir = CardinalDirection.East.ToVector();
			int2 southDir = CardinalDirection.South.ToVector();
			int2 westDir = CardinalDirection.West.ToVector();

			unexplored.Enqueue(startPosition);

			while (unexplored.Count > 0) {
				int2 pos = unexplored.Dequeue();

				if(visited.Contains(pos))
					continue;

				visited.Add(pos);
				dirtyChunks.MarkChunkDirtyAtWorldPosition(pos);
				LandTile tile = grid.GetTileData(pos);
				byte lowestPoint = tile.GetLowestPoint();
				int lowestWaterLevel = lowestPoint * WaterHeightStepsPerTileHeight;

				if (lowestWaterLevel + 1 > newWaterLevel) {
					tile.SetWaterLevel(0); //hide water 
					grid.SetTile(pos, tile);
					continue;
				}

				tile.SetWaterLevel(height);
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
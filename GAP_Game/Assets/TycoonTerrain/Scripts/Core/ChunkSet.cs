using System.Collections.Generic;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	/// <summary>
	/// Represents a collection or set of chunks that are marked as dirty.
	/// Marking chunks dirty means that their mesh representation will be updated at the end of this frame.
	/// </summary>
	public struct ChunkSet {

		private HashSet<int2> hashSet;
		private readonly int2 chunkSize;
		private int2 worldSize;

		public bool ContainsAny => hashSet.Count > 0;

		private IntBound worldBounds => new IntBound(new int2(0), worldSize - 1);

		public ChunkSet(int2 terrainSize, int chunkSize) {
			worldSize = terrainSize;
			this.chunkSize = new int2(chunkSize);
			hashSet = new HashSet<int2>();
		}

		/// <summary>
		/// Clear all the dirty chunks.
		/// </summary>
		public void Clear() {
			hashSet.Clear();
		}

		public IntBound GetChunkBounds(int2 chunkPosition) {
			return worldBounds.Intersection(new IntBound(chunkPosition * chunkSize, (chunkPosition + 1) * chunkSize - 1));
		}

		/// <summary>
		/// Gets the chunk positions of all dirty chunks.
		/// </summary>
		public IEnumerable<int2> GetChunkPositions() {
			var enumerator = hashSet.GetEnumerator();
			while (enumerator.MoveNext()) {
				yield return enumerator.Current;
			}
			enumerator.Dispose();
		}

		/// <summary>
		/// Marks are chunks dirty within the given bounds.
		/// </summary>
		/// <param name="bounds"></param>
		public void MarkRegionDirty(IntBound bounds) {
			//Expand bounds by 1 in all directions to make sure that neighboring chunks at boundaries re-render their cliffs properly
			MarkStrictRegionDirty(bounds.Expand(new int2(1)));
		}

		private void MarkStrictRegionDirty(IntBound bounds) {
			bounds = bounds.Intersection(worldBounds);

			int2 chunkMin = bounds.Min / chunkSize;
			int2 chunkMax = bounds.Max / chunkSize;

			for (int x = chunkMin.x; x <= chunkMax.x; x++) {
				for (int z = chunkMin.y; z <= chunkMax.y; z++) {
					hashSet.Add(new int2(x, z));
				}
			}
		}

		/// <summary>
		/// Marks the chunk dirty at the given tile position.
		/// </summary>
		/// <param name="tilePosition">The position in tile space.</param>
		public void MarkChunkDirtyAtWorldPosition(int2 tilePosition) {
			hashSet.Add(ToChunkPosition(tilePosition));
		}

		/// <summary>
		/// Marks the chunk dirty at the given chunk position.
		/// </summary>
		/// <param name="chunkPosition">The position in chunk space.</param>
		public void MarkChunkDirty(int2 chunkPosition) {
			hashSet.Add(chunkPosition);
		}

		private int2 ToChunkPosition(int2 worldPosition) {
			return worldPosition / chunkSize;
		}

		/// <summary>
		/// Marks all chunks of the terrain as dirty. Note that this can be expensive for large terrains!
		/// </summary>
		public void MarkAllChunksDirty() {
			MarkStrictRegionDirty(worldBounds);
		}
	}
}
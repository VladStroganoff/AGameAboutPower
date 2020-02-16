using System.Collections.Generic;
using TycoonTerrain.Core.TerrainOperations;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace TycoonTerrain.Core {
	/// <summary>
	/// Main component for the terrain. This is responsible for storing the internal map state and converting between world space and map coordinates
	/// </summary>
	[AddComponentMenu("Tycoon Tile/Tycoon Tile Map")]
	[DisallowMultipleComponent]
	public class TycoonTileMap : MonoBehaviour {
		[Tooltip("The number of tiles in the x direction")]
		public int Width = 250;
		[Tooltip("The number of tiles in the z direction")]
		public int Length = 250;
		[Tooltip("The number of tiles in both directions represented by a chunk.")]
		public int ChunkSize = 16;

		[Tooltip("The maximum number of steps tiles are allowed to be high.")]
		public byte MaxHeight = 60;

		[Tooltip("The world height that is represented by a single height step.")]
		public float WorldHeightStep = 1f;
		[Tooltip("The number of water height steps there are within one tile height step.")]
		public int WaterHeightStepsPerTileHeight = 4;

		/// <summary>
		/// A reference to the underlying terrain grid.
		/// </summary>
		[SerializeField] private TerrainGrid grid;

		/// <summary>
		/// Keeps track of the chunks that were modified during this frame.
		/// </summary>
		private ChunkSet dirtyChunks;

		/// <summary>
		/// Keeps track of all listeners that should receive events on chunk modification.
		/// </summary>
		private List<IChunkListener> chunkListeners;

		/// <summary>
		/// A reference to the underlying terrain type table.
		/// </summary>
		private TerrainTypeTable typeTable;

		/// <summary>
		/// The world height of a single water height step. This depends on the world height of a tile height step and the number of water height steps per tile height.
		/// </summary>
		public float WaterHeightStep {
			get { return WorldHeightStep / WaterHeightStepsPerTileHeight; }
		}

		/// <summary>
		/// The bounds of the entire terrain.
		/// </summary>
		public IntBound Bounds => grid.Bounds;

		internal TerrainGrid Grid => grid;

		public TerrainTypeTable TypeTable { get => typeTable; internal set => typeTable = value; }

		// Initialization
		void Awake() {
			grid = new TerrainGrid(Width, Length, MaxHeight, WaterHeightStepsPerTileHeight);
			typeTable = new TerrainTypeTable(new int2(Width, Length), Allocator.Persistent);
			grid.ResetData();
			dirtyChunks = new ChunkSet(new int2(Width, Length), ChunkSize);
			chunkListeners = new List<IChunkListener>();

			dirtyChunks.MarkAllChunksDirty();
		}

		private void OnValidate() {
			if (Width < 0)
				Width = 0;

			if (Length < 0)
				Length = 0;

			if (WorldHeightStep < 0)
				WorldHeightStep = 0;

			if (WaterHeightStepsPerTileHeight < 1)
				WaterHeightStepsPerTileHeight = 1;

			MaxHeight = (byte)Mathf.Clamp(MaxHeight, 0, 255);
		}

		void LateUpdate() {
			if (dirtyChunks.ContainsAny) {

				foreach (IChunkListener listener in chunkListeners) {
					Profiler.BeginSample("ChunkListener.OnUpdateChunks");
					listener.OnUpdateChunks(this, ref dirtyChunks);
					Profiler.EndSample();
				}
				dirtyChunks.Clear();
			}
		}

		//Dispose resources when destroyed
		void OnDestroy() {
			grid.Dispose();
			typeTable.Dispose();
		}

		/// <summary>
		/// Check if a certain position is within the map bounds.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>True if the position is within the map bounds, false otherwise.</returns>
		public bool IsInBounds(int2 pos) {
			return grid.IsInBounds(pos);
		}

		/// <summary>
		/// Schedule a paint operation that will be executed this frame.
		/// </summary>
		/// <typeparam name="T">The paint operation type.</typeparam>
		/// <param name="job">The paint operation.</param>
		public void SchedulePaintOperation<T>(T job) where T : struct, ITerrainPaintJob {
			Profiler.BeginSample("SchedulePaintOperation - Paint Job Execute");
			//TODO: Currently the operation is executed immediately, maybe schedule in Job System later?
			job.Execute(ref typeTable, ref dirtyChunks);
			Profiler.EndSample();
		}

		/// <summary>
		/// Schedule a terrain operation that will be executed this frame.
		/// </summary>
		/// <typeparam name="T">The terrain operation type.</typeparam>
		/// <param name="job">The terrain operation.</param>
		public void ScheduleOperation<T>(T job) where T : struct, ITerrainJob {
			Profiler.BeginSample("ScheduleOperation - Job Execute");
			//TODO: Currently the operation is executed immediately, maybe schedule in Job System later?
			job.Execute(ref grid, ref dirtyChunks);
			Profiler.EndSample();
		}

		/// <summary>
		/// Register a listener that will receive callbacks when chunks are marked dirty.
		/// </summary>
		/// <param name="listener">The chunk listener.</param>
		public void RegisterDirtyChunkListener(IChunkListener listener) {
			chunkListeners.Add(listener);
		}
	}
}
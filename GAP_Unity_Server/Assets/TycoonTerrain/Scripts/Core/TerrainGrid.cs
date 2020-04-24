using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	/// <summary>
	/// The terrain grid class is responsible for containing the LandTile data in a flattened 2D array.
	/// </summary>
	public struct TerrainGrid : IDisposable {
		public int Width;
		public int Length;
		public byte MaxHeight;

		public int WaterHeightStepsPerTileHeight;

		[SerializeField]
		public NativeArray<LandTile> map;

		public IntBound Bounds { get; }

		public byte MinHeight => 1;

		public TerrainGrid(int width, int length, byte maxHeight = 20, int waterHeightStepsPerTileHeight = 4, Allocator allocator = Allocator.Persistent) {
			Width = width;
			Length = length;
			map = new NativeArray<LandTile>(Width * Length, allocator);
			Bounds = new IntBound(int2.zero, new int2(Width - 1, Length - 1));
			MaxHeight = maxHeight;
			WaterHeightStepsPerTileHeight = waterHeightStepsPerTileHeight;
		}

        public void ResetData() {
			byte defaultHeight = (byte) (MaxHeight / 2);
			for (int i = 0; i < map.Length; i++) {
				map[i] = new LandTile(defaultHeight);
			}
		}

		public IntBound IntersectBound(IntBound bounds) {
			return Bounds.Intersection(bounds);
		}

		/// <summary>
		/// Check if a certain position is within the map bounds.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>True if the position is within the map bounds, false otherwise.</returns>
		public bool IsInBounds(int2 position) {
			return Bounds.Contains(position);
		}

		internal bool IsInBounds(int x, int z) {
			return Bounds.Contains(new int2(x, z));
		}

		internal LandTile GetTileData(int x, int z) {
			return map[x + z * Length];
		}

		internal LandTile GetTileData(int2 pos) {
			return map[pos.x + pos.y * Length];
		}

		public TileHandle GetTile(int x, int z) {
			if (x < 0 || x >= Width) {
				throw new ArgumentOutOfRangeException("x", $"x was {x}");
			}

			if (z < 0 || z >= Length) {
				throw new ArgumentOutOfRangeException("z", $"z was {z}");
			}

			return new TileHandle(this, new int2(x, z));
		}

		/// <summary>
		/// Gets a handle to the tile at <paramref name="pos"/>
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>The tile handle that points to the tile at <paramref name="pos"/>.</returns>
		public TileHandle GetTile(int2 pos) {
			return new TileHandle(this, pos);
		}

		public IEnumerable<TileHandle> GetTilesIn(IntBound bounds) {
			bounds = bounds.Intersection(Bounds);

			for (int x = bounds.Min.x; x <= bounds.Max.x; x++) {
				for (int z = bounds.Min.y; z <= bounds.Max.y; z++) {
					yield return new TileHandle(this, new int2(x, z));
				}
			}
		}

		public byte GetLowestInBounds(IntBound bounds) {
			bounds = bounds.Intersection(Bounds);

			return GetTilesIn(bounds).Min(t => t.GetData().GetLowestPoint());
		}

		public byte GetHighestInBounds(IntBound bounds) {
			bounds = bounds.Intersection(Bounds);

			return GetTilesIn(bounds).Max(t => t.GetData().GetHighestPoint());
		}

		public bool IsFlatInBounds(IntBound bounds) {
			bounds = bounds.Intersection(Bounds);

			//Just get any height within the bounds
			byte height = GetTile(bounds.Min).GetHeight(CornerIndex.NorthEast);

			foreach (TileHandle handle in GetTilesIn(bounds)) {
				if (handle.GetData().GetHighestPoint() != height || handle.GetData().GetLowestPoint() != height) {
					return false;
				}
			}

			return true;
		}

		public void SetTile(int2 pos, LandTile tile) {
			if (pos.x < 0 || pos.x >= Width) {
				throw new ArgumentOutOfRangeException("x");
			}

			if (pos.y < 0 || pos.y >= Length) {
				throw new ArgumentOutOfRangeException("z");
			}

			map[pos.x + pos.y * Length] = tile;
		}

		internal int IncreaseHeight(TileHandle tileHandle, CornerIndex? cornerIndex = null) {
			LandTile tile = tileHandle.GetData();

			int cost;
			if (cornerIndex == null) {
				if (tile.GetLowestPoint() >= MaxHeight) {
					return 0;
				}
				cost = tile.IncreaseHeight();
			}
			else {
				//Restrict height editing to the terrains limits
				if (tile.GetHeight(cornerIndex.Value) >= MaxHeight) {
					return 0;
				}

				cost = tile.IncreaseHeight(cornerIndex.Value);
			}

			var lowest = tile.GetLowestPoint();
			if (tile.WaterLevel <= lowest * WaterHeightStepsPerTileHeight) {
				tile.ClearWater();
			}
			SetTile(tileHandle.tilePosition, tile);

			return cost;
		}

		internal int DecreaseHeight(TileHandle tileHandle, CornerIndex? cornerIndex = null) {
			LandTile tile = tileHandle.GetData();

			int cost = 0;
			if (cornerIndex == null) {
				if (tile.GetHighestPoint() <= MinHeight) {
					return 0;
				}
				tile.DecreaseHeight();
			}
			else {
				//Restrict height editing to the terrains limits
				if (tile.GetHeight(cornerIndex.Value) <= MinHeight) {
					return 0;
				}

				cost = tile.DecreaseHeight(cornerIndex.Value);
			}

			SetTile(tileHandle.tilePosition, tile);

			return cost;
		}

		public void Dispose() {
			map.Dispose();
		}
	}
}
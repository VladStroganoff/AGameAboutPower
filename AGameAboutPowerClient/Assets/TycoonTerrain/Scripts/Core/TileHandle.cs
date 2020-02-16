using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	/// <summary>
	/// This struct is a utility struct that can be used as a pointer within the map grid. It holds a position and reference to the map and provides useful functions to interact with the underlying tile.
	/// User code should use this struct to interact with the underlying tile whenever possible.
	/// </summary>
	public struct TileHandle {
		internal static readonly Vector3[] cornerOffsets = {new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f)};

		public readonly int2 tilePosition;

		private readonly TerrainGrid grid;

		internal TerrainGrid Grid => grid;

		private int x => tilePosition.x;

		private int z => tilePosition.y;

		public bool IsInBounds {
			get { return grid.IsInBounds(tilePosition); }
		}

		public Vector3 CenterPosition {
			get { return new Vector3(tilePosition.x, 0, tilePosition.y); }
		}

		public float CenterSurfaceHeight {
			get {
				LandTile tile = grid.GetTileData(tilePosition);
				byte lowest = tile.GetLowestPoint();
				int centerHeight = lowest;

				//Case only 1 corner is lower than the others, this also includes the diagonally slanted case
				if (tile.NumberOfCornersAtHeight(lowest) == 1) {
					centerHeight = lowest + 1;
				}

				float height = centerHeight;

				//Case when tile is axis aligned sloped
				if (tile.IsAASlope) {
					height += 0.5f;
				}

				return height;
			}
		}

		public bool IsWaterLevelAboveSurface(int waterLevel) {
			return GetData().GetLowestPoint() * grid.WaterHeightStepsPerTileHeight < waterLevel;
		}

		public bool IsWaterLevelSurfaced {
			get { return GetData().GetLowestPoint() * grid.WaterHeightStepsPerTileHeight < GetData().WaterLevel; }
		}

		public bool IsWaterLevelBelowSurface {
			get { return !IsWaterLevelSurfaced; }
		}

		public bool IsCompletelySubmerged {
			get { return GetData().GetHighestPoint() * grid.WaterHeightStepsPerTileHeight < GetData().WaterLevel; }
		}

		public bool IsEdgeFlush(CardinalDirection direction) {
			int2 offset = direction.ToVector();
			int2 neighborPos = tilePosition + offset;

			//If the neighbour is outside the map, the edge is not flush
			if (!grid.IsInBounds(neighborPos))
				return false;

			TileHandle neighbour = grid.GetTile(neighborPos);

			CornerIndex originLeftCorner = CornerIndex.GetCornerOfDirection(direction, true);
			CornerIndex originRightCorner = CornerIndex.GetCornerOfDirection(direction, false);

			CornerIndex neighbourLeftCorner = CornerIndex.GetCornerOfDirection(direction.Inverse, true);
			CornerIndex neighbourRightCorner = CornerIndex.GetCornerOfDirection(direction.Inverse, false);

			//Edge is flush if both opposing corners are of equal height
			return GetHeight(originLeftCorner) == neighbour.GetHeight(neighbourRightCorner) &&
			       GetHeight(originRightCorner) == neighbour.GetHeight(neighbourLeftCorner);
		}

		public TileHandle([NotNull] TerrainGrid terrain, int2 position) {
			tilePosition = position;
			grid = terrain;
		}

		public LandTile GetData() {
			return grid.GetTileData(tilePosition);
		}

		internal Vector3 GetCornerPosition(CornerIndex cornerIndex) {
			return CenterPosition + GetCornerOffset(cornerIndex);
		}

		private Vector3 GetCornerOffset(CornerIndex cornerIndex) {
			if (!cornerIndex.IsValid) {
				throw new ArgumentException("Corner Index not valid", "cornerIndex");
			}

			if (!IsInBounds) {
				return cornerOffsets[cornerIndex.Index];
			}

			float height = GetData().GetHeight(cornerIndex);
			return cornerOffsets[cornerIndex.Index] + new Vector3(0, height, 0);
		}

		internal Vector3 GetBaseCornerOffset(CornerIndex cornerIndex) {
			return cornerOffsets[cornerIndex.Index];
		}

		public IEnumerable<TileHandle> Neighbours() {

			if (TryGetNeighbour(CardinalDirection.North, out TileHandle northHandle)) {
				yield return northHandle;
			}
			if (TryGetNeighbour(CardinalDirection.East, out TileHandle eastHandle)) {
				yield return eastHandle;
			}
			if (TryGetNeighbour(CardinalDirection.South, out TileHandle southHandle)) {
				yield return southHandle;
			}
			if (TryGetNeighbour(CardinalDirection.West, out TileHandle westHandle)) {
				yield return westHandle;
			}
		}

		public bool TryGetNeighbour(CardinalDirection direction, out TileHandle handle) {
			int2 neighborPosition = tilePosition + direction.ToVector();

			if (!grid.IsInBounds(neighborPosition)) {
				//Create an invalid TileHandle
				handle = new TileHandle(grid, neighborPosition);
				return false;
			}

			handle = grid.GetTile(neighborPosition);
			return true;
		}

		public TileHandle GetNeighbourOrDefault(CardinalDirection direction) {
			int2 offset = direction.ToVector();
			int2 neighborPosition = tilePosition + offset;

			if (!grid.IsInBounds(neighborPosition)) {
				//Create an invalid TileHandle
				return new TileHandle(grid, neighborPosition);
			}

			return grid.GetTile(neighborPosition);
		}

		/// <summary>
		/// Gets the height of the specified tile corner.
		/// </summary>
		/// <param name="cornerIndex">The corner index.</param>
		/// <returns>The height of the corner.</returns>
		public byte GetHeight(CornerIndex cornerIndex) {
			return GetData().GetHeight(cornerIndex);
		}
	}
}
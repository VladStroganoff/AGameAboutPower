using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	/// <summary>
	/// LandTile struct is responsible for containing the actual height data for each corner, as well as a water level.
	/// It provides some utility properties and functions on top of this.
	/// </summary>
	[System.Serializable]
	public struct LandTile : IEquatable<LandTile> {
		[SerializeField] private byte cornerNE;
		[SerializeField] private byte cornerSE;
		[SerializeField] private byte cornerSW;
		[SerializeField] private byte cornerNW;
		[SerializeField] private ushort waterHeight;

		/// <summary>
		/// Whether this tile is completely flat (eg. all corner heights are equal).
		/// </summary>
		public bool IsFlat {
			get { return cornerNE == cornerSE && cornerNE == cornerSW && cornerNE == cornerNW; }
		}

		public bool IsAASlope {
			get {
				return !IsFlat && ((cornerNE == cornerNW && cornerSE == cornerSW) || (cornerNE == cornerSE && cornerNW == cornerSW));
			}
		}

		/// <summary>
		/// Gets the water level in this tile.
		/// </summary>
		public ushort WaterLevel {
			get { return waterHeight; }
		}

		/// <summary>
		/// Gets the heights of all corners in the form of a vector, in the order (x = NorthEast, y = SouthEast, z = SouthWest, w = NorthWest)
		/// </summary>
		public int4 Heights => new int4(cornerNE, cornerSE, cornerSW, cornerNW);

		public LandTile(byte ne, byte se, byte sw, byte nw, ushort water = 0) {
			cornerNE = ne;
			cornerSE = se;
			cornerSW = sw;
			cornerNW = nw;
			waterHeight = water;
		}

		public LandTile(byte height) {
			cornerNE = height;
			cornerSE = height;
			cornerSW = height;
			cornerNW = height;
			waterHeight = 0;
		}

		/// <summary>
		/// Increases the height of the corner of the specified corner index by one. Handles neighboring corner height increases as well.
		/// </summary>
		/// <param name="cornerIndex">The index of the corner.</param>
		/// <returns>The total number of height adjustments by all corners.</returns>
		public int IncreaseHeight(CornerIndex cornerIndex) {
			byte height = GetHeight(cornerIndex);
			int cost = 0;

			if (height + 1 <= 255) {
				height++;
				cost++;
			}

			SetHeight(cornerIndex, height);

			//Make sure that the other corners of this tile dont differ more than 1 height between neighbours
			CornerIndex leftCorner = cornerIndex.NeighbourCounterClockwise;
			if (GetHeight(leftCorner) < height - 1) {
				cost += IncreaseHeight(leftCorner);
			}

			CornerIndex rightCorner = cornerIndex.NeighbourClockwise;
			if (GetHeight(rightCorner) < height - 1) {
				cost += IncreaseHeight(rightCorner);
			}

			return cost;
		}

		/// <summary>
		/// Increases the height of the corners associated with the specified edge.
		/// </summary>
		/// <param name="edge">The edge.</param>
		/// <returns></returns>
		public int IncreaseHeight(CardinalDirection edge) {
			return IncreaseHeight(CornerIndex.GetCornerOfDirection(edge, true)) +
			       IncreaseHeight(CornerIndex.GetCornerOfDirection(edge, false));
		}

		/// <summary>
		/// Increases the height of the lowest corners of the tile by one.
		/// </summary>
		/// <returns>The total number of height adjustments by all corners.</returns>
		public int IncreaseHeight() {
			int cost = 0;
			byte lowest= (byte)(GetLowestPoint() + 1);
			if (cornerNE < lowest) {
				cornerNE = lowest;
				cost += lowest - cornerNE;
			}
			if (cornerSE < lowest) {
				cornerSE = lowest;
				cost += lowest - cornerSE;
			}
			if (cornerSW < lowest) {
				cornerSW = lowest;
				cost += lowest - cornerSW;
			}
			if (cornerNW < lowest) {
				cornerNW = lowest;
				cost += lowest - cornerNW;
			}

			return cost;
		}

		/// <summary>
		/// Decreases the height of the highest corners of the tile by one.
		/// </summary>
		/// <returns>The total number of height adjustments by all corners.</returns>
		public void DecreaseHeight() {
			byte heighest = (byte)(GetHighestPoint() - 1);
			if (cornerNE > heighest) {
				cornerNE = heighest;
			}
			if (cornerSE > heighest) {
				cornerSE = heighest;
			}
			if (cornerSW > heighest) {
				cornerSW = heighest;
			}
			if (cornerNW > heighest) {
				cornerNW = heighest;
			}
		}

		public int DecreaseHeight(CornerIndex cornerIndex) {
			byte height = GetHeight(cornerIndex);
			int cost = 0;

			if (height - 1 >= 0) {
				height--;
				cost++;
			}

			SetHeight(cornerIndex, height);

			//Make sure that the other corners of this tile dont differ more than 1 height between neighbours
			CornerIndex leftCorner = cornerIndex.NeighbourCounterClockwise;
			if (GetHeight(leftCorner) > height + 1) {
				cost += DecreaseHeight(leftCorner);
			}

			CornerIndex rightCorner = cornerIndex.NeighbourClockwise;
			if (GetHeight(rightCorner) > height + 1) {
				cost += DecreaseHeight(rightCorner);
			}

			return cost;
		}

		/// <summary>
		/// Sets the height of all corners of this tile to the minimum of 'min' the existing height.
		/// </summary>
		/// <param name="min">The height at which all corners should minimally be set.</param>
		public void MinHeight(byte min) {
			int4 result = math.min(Heights, new int4((int)min));
			SetHeights(result);
		}

		/// <summary>
		/// Sets the height of all corners of this tile to the maximum of 'max' the existing height.
		/// </summary>
		/// <param name="max">The height at which all corners should maximally be set.</param>
		public void MaxHeight(byte max) {
			int4 result = math.max(Heights, new int4((int)max));
			SetHeights(result);
		}

		/// <summary>
		/// Sets the height of all corners of this tile to the maximum of 'max' the existing height.
		/// </summary>
		/// <param name="max">The height at which all corners should maximally be set.</param>
		public void MaxHeight(int4 max) {
			max = math.clamp(max, new int4(0), new int4(255));
			int4 result = math.max(Heights, max);
			SetHeights(result);
		}

		/// <summary>
		/// Sets all corner heights to the given height
		/// </summary>
		/// <param name="height">The height.</param>
		public void SetHeight(byte height) {
			cornerNE = height;
			cornerSE = height;
			cornerSW = height;
			cornerNW = height;
		}

		private void SetHeight(CornerIndex corner, byte height) {
			switch (corner.CornerDirection) {
				case CornerDirection.NorthEast:
					cornerNE = height;
					return;
				case CornerDirection.SouthEast:
					cornerSE = height;
					return;
				case CornerDirection.SouthWest:
					cornerSW = height;
					return;
				case CornerDirection.NorthWest:
					cornerNW = height;
					return;
			}

			throw new ArgumentException("Invalid corner direction");
		}

		internal void SetHeights(int4 heights) {
			heights = math.clamp(heights, new int4(0), new int4(255));

			cornerNE = (byte)heights.x;
			cornerSE = (byte)heights.y;
			cornerSW = (byte)heights.z;
			cornerNW = (byte)heights.w;
		}

		public byte GetHeight(CornerIndex corner) {
			switch (corner.CornerDirection) {
				case CornerDirection.NorthEast:
					return cornerNE;
				case CornerDirection.SouthEast:
					return cornerSE;
				case CornerDirection.SouthWest:
					return cornerSW;
				case CornerDirection.NorthWest:
					return cornerNW;
			}

			throw new ArgumentException("Invalid corner direction");
		}

		internal int NumberOfCornersAtHeight(byte height) {
			int count = 0;
			count += cornerNE == height ? 1 : 0;
			count += cornerSE == height ? 1 : 0;
			count += cornerSW == height ? 1 : 0;
			count += cornerNW == height ? 1 : 0;

			return count;
		}

		/// <summary>
		/// Gets the height of the lowest corner.
		/// </summary>
		/// <returns>The height of the lowest corner.</returns>
		public byte GetLowestPoint() {
			byte lowest = cornerNE;
			if (cornerSE < lowest) {
				lowest = cornerSE;
			}
			if (cornerSW < lowest) {
				lowest = cornerSW;
			}
			if (cornerNW < lowest) {
				lowest = cornerNW;
			}

			return lowest;
		}

		/// <summary>
		/// Gets the height of the highest corner.
		/// </summary>
		/// <returns>The height of the highest corner.</returns>
		public byte GetHighestPoint() {
			byte highest = cornerNE;
			if (cornerSE > highest) {
				highest = cornerSE;
			}
			if (cornerSW > highest) {
				highest = cornerSW;
			}
			if (cornerNW > highest) {
				highest = cornerNW;
			}

			return highest;
		}

		public override bool Equals(object obj) {
			return obj is LandTile && Equals((LandTile)obj);
		}

		public bool Equals(LandTile other) {
			return cornerNE == other.cornerNE &&
				   cornerSE == other.cornerSE &&
				   cornerSW == other.cornerSW &&
				   cornerNW == other.cornerNW &&
				   waterHeight == other.waterHeight;
		}

		public override int GetHashCode() {
			var hashCode = -1532490875;
			hashCode = hashCode * -1521134295 + cornerNE.GetHashCode();
			hashCode = hashCode * -1521134295 + cornerSE.GetHashCode();
			hashCode = hashCode * -1521134295 + cornerSW.GetHashCode();
			hashCode = hashCode * -1521134295 + cornerNW.GetHashCode();
			hashCode = hashCode * -1521134295 + waterHeight.GetHashCode();
			return hashCode;
		}

		public void SetWaterLevel(ushort waterLevel) {
			waterHeight = waterLevel;
		}

		internal void ClearWater() {
			waterHeight = 0;
		}
	}

	public enum CornerDirection {
		NorthEast,
		SouthEast,
		SouthWest,
		NorthWest
	}
}
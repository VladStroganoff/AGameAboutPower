using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	/// <summary>
	/// Represents the index of a single corner of a tile.
	/// </summary>
	public struct CornerIndex : IEquatable<CornerIndex> {
		public static readonly CornerIndex NorthEast = new CornerIndex(CornerDirection.NorthEast);
		public static readonly CornerIndex SouthEast = new CornerIndex(CornerDirection.SouthEast);
		public static readonly CornerIndex SouthWest = new CornerIndex(CornerDirection.SouthWest);
		public static readonly CornerIndex NorthWest = new CornerIndex(CornerDirection.NorthWest);
		public static readonly CornerIndex Invalid = new CornerIndex(-1);

		private static readonly int2[] offsets = {new int2(1,1), new int2(1,0), new int2(0,0), new int2(0,1)};

		private const int NEIGHBOUR_CLOCKWISE = 1;
		private const int NEIGHBOUR_COUNTERCLOCKWISE = 3;
		private const int NEIGHBOUR_OPPOSITE = 2;

		private readonly byte index;

		public bool IsValid {
			get { return index == 0 || index == 1 || index == 2 || index == 3; }
		}

		internal CornerIndex(int rawIndex) {
			if (rawIndex < 0) {
				index = 255;
			}

			index = (byte)(rawIndex % 4);
		}

		public CornerIndex(CornerDirection direction) {
			switch (direction) {
				case CornerDirection.NorthEast:
					index = 0;
					return;
				case CornerDirection.SouthEast:
					index = 1;
					return;
				case CornerDirection.SouthWest:
					index = 2;
					return;
				case CornerDirection.NorthWest:
					index = 3;
					return;
			}

			throw new ArgumentException("Direction not valid");
		}

		internal int Index {
			get { return index; }
		}

		public static IEnumerable<CornerIndex> All {
			get {
				yield return NorthEast;
				yield return SouthEast;
				yield return SouthWest;
				yield return NorthWest;
			}
		}

		/// <summary>
		/// Returns a CornerIndex that is associated with a given cardinal direction.
		/// </summary>
		/// <param name="direction">The direction pointing outside from the center of the tile</param>
		/// <param name="left">When looking from the center of a tile, whether to return the left (true) or right (false) corner in this direction</param>
		/// <returns></returns>
		public static CornerIndex GetCornerOfDirection(CardinalDirection direction, bool left) {
			if (direction == CardinalDirection.North) {
				return left ? NorthWest : NorthEast;
			}

			if (direction == CardinalDirection.East) {
				return left ? NorthEast : SouthEast;
			}

			if (direction == CardinalDirection.South) {
				return left ? SouthEast : SouthWest;
			}

			if (direction == CardinalDirection.West) {
				return left ? SouthWest : NorthWest;
			}

			return Invalid;
		}

		internal CornerDirection CornerDirection {
			get {
				switch (index) {
					case 0:
						return CornerDirection.NorthEast;
					case 1:
						return CornerDirection.SouthEast;
					case 2:
						return CornerDirection.SouthWest;
					case 3:
						return CornerDirection.NorthWest;
				}

				throw new InvalidOperationException();
			}
		}

		public int2 ToVertexOffset() {
			return offsets[index];
		}

		public CornerIndex NeighbourClockwise {
			get { return new CornerIndex(index + NEIGHBOUR_CLOCKWISE); }
		}

		public CornerIndex NeighbourCounterClockwise {
			get { return new CornerIndex(index + NEIGHBOUR_COUNTERCLOCKWISE); }
		}

		public CornerIndex NeighbourOpposite {
			get { return new CornerIndex(index + NEIGHBOUR_OPPOSITE); }
		}

		public override bool Equals(object obj) {
			return obj is CornerIndex && Equals((CornerIndex)obj);
		}

		public bool Equals(CornerIndex other) {
			return index == other.index;
		}

		public override int GetHashCode() {
			return -1982729373 + index.GetHashCode();
		}

		public static bool operator ==(CornerIndex index1, CornerIndex index2) {
			return index1.Equals(index2);
		}

		public static bool operator !=(CornerIndex index1, CornerIndex index2) {
			return !(index1 == index2);
		}
	}
}
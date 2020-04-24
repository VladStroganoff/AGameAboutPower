using System;
using Unity.Mathematics;

namespace TycoonTerrain.Core {
	/// <summary>
	/// Represents a north/east/south/west direction.
	/// </summary>
	public struct CardinalDirection : IEquatable<CardinalDirection> {
		public static CardinalDirection North => new CardinalDirection(0);
		public static CardinalDirection East => new CardinalDirection(1);
		public static CardinalDirection South => new CardinalDirection(2);
		public static CardinalDirection West => new CardinalDirection(3);

		private static readonly int2[] vectors = {
			new int2(0, 1),
			new int2(1, 0),
			new int2(0, -1),
			new int2(-1, 0)
		};

		private readonly int directionIndex;

		internal CardinalDirection(int index) {
			if (index < 0 || index > 3) {
				throw new ArgumentException("Invalid Cardinal Direction Index", nameof(index));
			}

			directionIndex = index;
		}

		/// <summary>
		/// Returns a direction from an offset vector.
		/// </summary>
		/// <param name="vector">The offset vector.</param>
		public CardinalDirection(int2 vector) {
			directionIndex = -1;

			bool wasSet = false;
			for (int i = 0; i < vectors.Length; i++) {
				if (vector.x == vectors[i].x && vector.y == vectors[i].y) {
					directionIndex = i;
					wasSet = true;
				}
			}

			if (!wasSet) {
				throw new ArgumentException("Invalid direction", nameof(vector));
			}
		}

		/// <summary>
		/// Returns a direction from the point of view of a corner index, facing outwards.
		/// </summary>
		/// <param name="corner">The corner index.</param>
		/// <param name="left">Whether to return the left or right direction.</param>
		public static CardinalDirection GetDirectionFromCorner(CornerIndex corner, bool left) {
			if (left) {
				return new CardinalDirection(corner.Index);
			}

			return new CardinalDirection(corner.NeighbourClockwise.Index);
		}

		/// <summary>
		/// Returns the offset in tile positions that is represented by the direction.
		/// </summary>
		public int2 ToVector() {
			return vectors[directionIndex];
		}

		/// <summary>
		/// Returns the opposite direction.
		/// </summary>
		/// <example>North.Inverse returns South.</example>
		public CardinalDirection Inverse => new CardinalDirection((directionIndex + 2) % 4);

		/// <summary>
		/// Returns the direction when one would turn 90 degrees to the left from the current direction.
		/// </summary>
		/// <example>North.RotateLeft returns West.</example>
		public CardinalDirection RotateLeft => new CardinalDirection((directionIndex + 3) % 4);

		/// <summary>
		/// Returns the direction when one would turn 90 degrees to the right from the current direction.
		/// </summary>
		/// <example>North.RotateRight returns East.</example>
		public CardinalDirection RotateRight => new CardinalDirection((directionIndex + 1) % 4);

		public bool Equals(CardinalDirection other) {
			return directionIndex == other.directionIndex;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}

			return obj is CardinalDirection other && Equals(other);
		}

		public override int GetHashCode() {
			return directionIndex;
		}

		public static bool operator ==(CardinalDirection left, CardinalDirection right) {
			return left.Equals(right);
		}

		public static bool operator !=(CardinalDirection left, CardinalDirection right) {
			return !left.Equals(right);
		}
	}
}
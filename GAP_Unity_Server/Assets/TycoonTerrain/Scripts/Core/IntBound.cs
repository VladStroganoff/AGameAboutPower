using System;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	public struct IntBound : IEquatable<IntBound> {
		public int2 Min { get; }
		public int2 Max { get; }

		public IntBound(int2 min, int2 max) : this() {
			Min = min;
			Max = max;
		}

		public bool Contains(int2 pos) {
			return pos.x >= Min.x && pos.y >= Min.y && pos.x <= Max.x && pos.y <= Max.y;
		}

		public bool Intersects(IntBound bounds) {
			return Min.x <= bounds.Max.x && Max.x >= bounds.Min.x && Min.y <= bounds.Max.y && Max.y >= bounds.Min.y;
		}

		public IntBound Intersection(IntBound other) {
			return new IntBound(new int2(Mathf.Max(Min.x, other.Min.x), Mathf.Max(Min.y, other.Min.y)), new int2(Mathf.Min(Max.x, other.Max.x), Mathf.Min(Max.y, other.Max.y)));
		}

		public IntBound Expand(int2 expansion) {
			return new IntBound(Min - expansion, Max + expansion);
		}

		public IntBound Expand(CardinalDirection direction, int amount) {
			int2 vector = direction.ToVector();

			int2 expansion = vector * amount;

			return new IntBound(math.min(Min, Min + expansion), math.max(Max, Max + expansion));
		}

		public int Distance(int2 pos) {
			if (Contains(pos)) {
				return 0;
			}

			int2 difMin = math.abs(Min - pos);
			int2 difMax = math.abs(Max - pos);

			int2 d = math.min(difMin, difMax);

			return d.x + d.y;
		}

		public bool Equals(IntBound other) {
			return Min.Equals(other.Min) && Max.Equals(other.Max);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) {
				return false;
			}

			return obj is IntBound other && Equals(other);
		}

		public override int GetHashCode() {
			unchecked {
				return (Min.GetHashCode() * 397) ^ Max.GetHashCode();
			}
		}

		public static bool operator ==(IntBound left, IntBound right) {
			return left.Equals(right);
		}

		public static bool operator !=(IntBound left, IntBound right) {
			return !left.Equals(right);
		}
	}
}
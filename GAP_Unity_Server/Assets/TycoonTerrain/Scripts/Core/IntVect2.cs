using System;

public struct IntVect2 : IEquatable<IntVect2> {
	public static readonly IntVect2 North = new IntVect2(0, 1);
	public static readonly IntVect2 East = new IntVect2(1, 0);
	public static readonly IntVect2 South = new IntVect2(0, -1);
	public static readonly IntVect2 West = new IntVect2(-1, 0);
	public static readonly IntVect2 Zero = new IntVect2(0, 0);

	private int z;
	private int x;

	public int X {
		get {
			return x;
		}

		set {
			x = value;
		}
	}

	public int Z {
		get {
			return z;
		}

		set {
			z = value;
		}
	}

	public IntVect2(int x, int z) {
		this.x = x;
		this.z = z;
	}

	public bool Equals(IntVect2 other) {
		return z == other.z && x == other.x;
	}

	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) {
			return false;
		}

		return obj is IntVect2 && Equals((IntVect2) obj);
	}

	public override int GetHashCode() {
		unchecked {
			return (z * 397) ^ x;
		}
	}

	public static bool operator ==(IntVect2 left, IntVect2 right) {
		return left.Equals(right);
	}

	public static bool operator !=(IntVect2 left, IntVect2 right) {
		return !left.Equals(right);
	}

	public static IntVect2 operator +(IntVect2 left, IntVect2 right) {
		return new IntVect2(left.x + right.x, left.z + right.z);
	}
}
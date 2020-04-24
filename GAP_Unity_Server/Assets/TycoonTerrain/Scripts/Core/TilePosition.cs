using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	public struct TilePosition {

		public static int2 WorldToTilePosition(Vector3 worldPos) {
			int xi = Mathf.RoundToInt(worldPos.x);
			int zi = Mathf.RoundToInt(worldPos.z);

			return new int2(xi, zi);
		}
	}
}
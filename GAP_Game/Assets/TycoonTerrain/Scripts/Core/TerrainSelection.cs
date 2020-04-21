using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	/// <summary>
	/// Represents a selection of the tycoon tile terrain. This is used with the included tools and previewers.
	/// </summary>
	public struct TerrainSelection {
		public CornerIndex CornerIndex;
		public TileHandle? handle;
		public int Size;
		public TycoonTileMap Terrain;

		public bool HasSelection => handle != null;

		public bool IsCenter => CornerIndex == CornerIndex.Invalid;

		public int2 Position => handle.HasValue ? handle.Value.tilePosition : new int2(-1, -1);

		public Vector3 WorldPosition {
			get {
				if (!HasSelection) {
					return Vector3.zero;
				}

				if (IsCenter) {
					float avg = handle.Value.CenterSurfaceHeight * Terrain.WorldHeightStep;
					Vector3 centerPositionAverage = handle.Value.CenterPosition + new Vector3(0, avg, 0);

					return Terrain.transform.TransformPoint(centerPositionAverage);
				}

				Vector3 cornerPosition = handle.Value.GetCornerPosition(CornerIndex);
				float height = cornerPosition.y * Terrain.WorldHeightStep;

				return Terrain.transform.TransformPoint(new Vector3(cornerPosition.x, height, cornerPosition.z));
			}
		}

		public IntBound Bounds {
			get {
				int2 offset = new int2(- (Size - 1) / 2, -(Size - 1) / 2);
				return new IntBound(Position + offset, Position + offset + new int2(Size - 1, Size - 1));
			}
		}

		public void Clear() {
			handle = null;
			CornerIndex = CornerIndex.Invalid;
		}
	}
}
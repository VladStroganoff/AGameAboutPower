using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	public static class TerrainHelper {
		private const float tileSize = 1f;

		public static bool Raycast(Ray ray, TycoonTileMap terrain, out RaycastHit hit, float distance) {
			Vector3 terrainDimensions = new Vector3(terrain.Width, terrain.MaxHeight * terrain.WorldHeightStep, terrain.Length);
			Bounds terrainBounds = new Bounds(terrain.transform.position + (terrainDimensions / 2f), terrainDimensions);
			TerrainGrid grid = terrain.Grid;

			if (!terrainBounds.IntersectRay(ray, out distance)) {
				hit = default;
				return false;
			}
			
			
			float dmin = (terrainBounds.min.x - ray.origin.x) / ray.direction.x;
			float dmax = (terrainBounds.max.x - ray.origin.x) / ray.direction.x;
			float d = math.min(dmin, dmax);
			Vector3 p = ray.GetPoint(d);
			int2 tilePosition = new int2(Mathf.FloorToInt(p.x / tileSize), Mathf.FloorToInt(p.z / tileSize));

			while (grid.IsInBounds(tilePosition) && terrainBounds.Contains(p)) {
				Debug.Log("Evaluating " + tilePosition);

				if (Raycast(ray, grid.GetTile(tilePosition), out hit, distance)) {
					return true;
				}

				float2 dt = GetNextPosition(tilePosition, p, ray.direction);
				d += math.min(dt.x, dt.y);
				p = ray.GetPoint(d);
				tilePosition = new int2(Mathf.FloorToInt(p.x / tileSize), Mathf.FloorToInt(p.z / tileSize));
			}
			hit = default;
			return false;
		}

		public static bool Raycast(Ray ray, TileHandle tile, out RaycastHit hit, float distance) {
			float height = 1;
			Debug.Log("Evaluating " + tile.tilePosition);

			Bounds bounds = new Bounds(tile.CenterPosition + new Vector3(0, height / 2, 0), new Vector3(1, height, 1));

			if (!bounds.IntersectRay(ray, out distance)) {
				hit = default;
				return false;
			}

			hit = default;
			return false;
		}

		private static float2 GetNextPosition(int2 tilePosition, float3 p, float3 rayDirection) {
			float2 sign = math.@select(0f, -1f, rayDirection.xz > 0);
			return ((tilePosition + sign) * tileSize - p.xz) / rayDirection.xz;
		}
	}
}
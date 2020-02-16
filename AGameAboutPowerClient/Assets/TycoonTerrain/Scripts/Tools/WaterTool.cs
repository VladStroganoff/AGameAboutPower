using TycoonTerrain.Core;
using TycoonTerrain.Core.TerrainOperations;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace TycoonTerrain {
	/// <summary>
	/// The water tool is used to create, modify and delete water bodies on the terrain.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class WaterTool : MonoBehaviour {
		[Tooltip("The maximum distance this tool can reach for terrain selection.")]
		public float MaxRayDistance;
		[Tooltip("This event is fired every time a water water is created, modified or deleted.")]
		public UnityEvent OnWaterPlacedEvent;

		/// <summary>
		/// Reference to the camera.
		/// </summary>
		private Camera cam;

		private RaycastHit lastHit;
		private bool lastHitAnything;
		private float lastWaterWorldHeight;

		private void Start() {
			cam = GetComponent<Camera>();
		}

		private void Update() {
			Raycast();
		}

		public bool GetLastRaycastHit(out RaycastHit hit, out float waterHeight) {
			if (!lastHitAnything) {
				hit = default;
				waterHeight = 0f;
				return false;
			}

			hit = lastHit;
			waterHeight = lastWaterWorldHeight;
			return true;
		}

		private void Raycast() {
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, MaxRayDistance)) {
				Vector3 pos = hit.point;
				TycoonTileMap tt = hit.collider.GetComponentInParent<TycoonTileMap>();
				if (tt == null) {
					lastHitAnything = false;
					return;
				}

				pos = tt.transform.InverseTransformPoint(pos);

				int x = Mathf.RoundToInt(pos.x);
				int z = Mathf.RoundToInt(pos.z);
				int2 tilePosition = new int2(x, z);
				int height = GetWaterHeight(tt, pos.y);
				var grid = tt.Grid;

				if (!grid.IsInBounds(tilePosition)) {
					lastHitAnything = false;
					return;
				}

				var tile = grid.GetTileData(tilePosition);
				if (tile.WaterLevel == height) {
					lastHitAnything = false;
					return;
				}

				lastHit = hit;

				lastWaterWorldHeight = height * tt.WaterHeightStep;
				lastHitAnything = height % tt.WaterHeightStepsPerTileHeight != 0;

				if(!Input.GetMouseButtonDown(0) || (!lastHitAnything && !tile.IsFlat))
					return;

				var tileHandle = grid.GetTile(tilePosition);

				//If we clicked a flat tile that is submerged
				if (tile.IsFlat && tileHandle.IsCompletelySubmerged) {
					tt.ScheduleOperation(new ClearWaterBodyOperation(tileHandle));
					OnWaterPlacedEvent.Invoke();
				}

				if (grid.IsInBounds(tilePosition) && tile.WaterLevel != height) {
					LandTile landTile = grid.GetTileData(tilePosition);
					int oldWaterLevel = landTile.WaterLevel;
					if (oldWaterLevel < height) {
						tt.ScheduleOperation(new CreateWaterBodyFloodOperation(tilePosition, height, grid.WaterHeightStepsPerTileHeight));
					}
					else {
						NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);
						tt.ScheduleOperation(new GetTilesInWaterBody(tilePosition, tiles));
						tt.ScheduleOperation(new SetWaterLevelOperation(tiles, (ushort)height));
					}
					OnWaterPlacedEvent.Invoke();
				}

			}
			else {
				lastHitAnything = false;
			}
		}

		private int GetWaterHeight(TycoonTileMap terrain, float height) {
			int result = Mathf.RoundToInt(height / terrain.WaterHeightStep);

			return Mathf.Clamp(result, 0, terrain.MaxHeight * terrain.WaterHeightStepsPerTileHeight);
		}
	}
}
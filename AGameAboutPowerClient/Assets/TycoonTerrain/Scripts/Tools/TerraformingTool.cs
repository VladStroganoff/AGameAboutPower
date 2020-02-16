using TycoonTerrain.Core;
using TycoonTerrain.Core.TerrainOperations;
using UnityEngine;
using UnityEngine.Events;

namespace TycoonTerrain {
	/// <summary>
	/// This is the terraforming tool.
	/// This class is responsible for scheduling terrain modifications based on selection changes and user input.
	/// </summary>
	[RequireComponent(typeof(TycoonTileRaycaster))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Tycoon Tile/Tools/Terraforming Tool")]
	public class TerraformingTool : MonoBehaviour {
		public bool UseSmooth;
		public UnityEvent OnTerraformEvent;

		private TerrainSelection selection;
		private TycoonTileRaycaster raycaster;

		//Mouse position of previous frame while dragging (in screen space).
		private Vector3 lastdragPosition;

		private void Awake() {
			raycaster = GetComponent<TycoonTileRaycaster>();
		}

		private void Update() {
			//As soon as we press the left mouse button, lock the selection
			if (Input.GetMouseButtonDown(0)) {
				lastdragPosition = Input.mousePosition;
				raycaster.LockSelection();
			}

			//When we release the left mouse button, unlock the selection
			if (Input.GetMouseButtonUp(0)) {
				raycaster.UnlockSelection();
			}

			//While the left mouse button is held down, handle dragging such that terrain can be terraformed.
			if (Input.GetMouseButton(0) && selection.HasSelection) {
				HandleDrag();
			}

			Debug.DrawLine(selection.WorldPosition, raycaster.WorldToScreenPoint(Input.mousePosition), Color.red);
		}

		private void OnEnable() {
			raycaster.OnSelectionChangedEvent.AddListener(OnSelectionChanged);
		}

		private void OnDisable() {
			selection.Clear();
			raycaster.OnSelectionChangedEvent.RemoveListener(OnSelectionChanged);
		}

		private void OnSelectionChanged(TerrainSelection newSelection) {
			selection = newSelection;
		}

		private void HandleDrag() {
			Vector3 mousePosition = Input.mousePosition;
			Vector3 selectionScreenPosition = raycaster.WorldToScreenPoint(selection.WorldPosition);

			//Determine how much the mouse has to move in screen space such that a distance of WorldHeightStep has been travelled in world space.
			float screenDeltaPerStep = Vector3.Distance(selectionScreenPosition, raycaster.WorldToScreenPoint(selection.WorldPosition + Vector3.up * selection.Terrain.WorldHeightStep));

			//Enforce minimal delta of 10 pixels to prevent unexpected behavior
			if (screenDeltaPerStep < 10f) {
				selectionScreenPosition = lastdragPosition;
				screenDeltaPerStep = Mathf.Max(screenDeltaPerStep, 10f);
			}

			if (Mathf.Abs(mousePosition.y - lastdragPosition.y) < screenDeltaPerStep) {
				return;
			}

			bool draggedUp = mousePosition.y > lastdragPosition.y;
			lastdragPosition = mousePosition;

			if (draggedUp) {
				//Check if terrain can still be increased
				if (selection.Terrain.Grid.GetLowestInBounds(selection.Bounds) < selection.Terrain.Grid.MaxHeight) {
					IncreaseHeight();
					OnTerraformEvent.Invoke();
				}
			}
			else {
				//Check if terrain can still be lowered
				if (selection.Terrain.Grid.GetHighestInBounds(selection.Bounds) > selection.Terrain.Grid.MinHeight) {
					DecreaseHeight();
					OnTerraformEvent.Invoke();
				}
			}
		}

		/// <summary>
		/// Increases the height of the terrain in the current selection
		/// </summary>
		private void IncreaseHeight() {
			if (!selection.HasSelection) {
				return;
			}

			if (selection.Size > 1 || selection.IsCenter) {
				if (UseSmooth) {
					selection.Terrain.ScheduleOperation(new IncreaseHeightSmoothConnectedOperation(selection.Bounds));
				}
				else {
					selection.Terrain.ScheduleOperation(new IncreaseHeightCliffOperation(selection.Bounds));
				}
			}
			else {
				//If only a single corner is selected
				selection.Terrain.ScheduleOperation(new IncreaseHeightCorner(selection.Position, selection.CornerIndex));
			}
		}

		/// <summary>
		/// Decreases the height of the terrain in the current selection
		/// </summary>
		private void DecreaseHeight() {
			if (!selection.HasSelection) {
				return;
			}

			if (selection.Size > 1 || selection.IsCenter) {
				if (UseSmooth) {
					selection.Terrain.ScheduleOperation(new DecreaseHeightSmoothConnectedOperation(selection.Bounds));
				}
				else {
					selection.Terrain.ScheduleOperation(new DecreaseHeightCliffOperation(selection.Bounds));
				}
			}
			else {
				//If only a single corner is selected
				selection.Terrain.ScheduleOperation(new DecreaseHeightCorner(selection.Position, selection.CornerIndex));
			}
		}
	}
}
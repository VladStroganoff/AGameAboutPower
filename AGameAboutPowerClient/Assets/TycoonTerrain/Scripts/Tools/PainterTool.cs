using TycoonTerrain.Core;
using TycoonTerrain.Core.PaintOperations;
using UnityEngine;

/// <summary>
/// This is the painting tool.
/// This class is responsible for scheduling painting operations based on selection changes and user input.
/// </summary>
[RequireComponent(typeof(TycoonTileRaycaster))]
[DisallowMultipleComponent]
[AddComponentMenu("Tycoon Tile/Tools/Terraforming Tool")]
public class PainterTool : MonoBehaviour
{
	/// <summary>
	/// The last selection changed event selection.
	/// </summary>
	private TerrainSelection selection;

	/// <summary>
	/// Reference to the raycaster.
	/// </summary>
	private TycoonTileRaycaster raycaster;

	[Tooltip("The id of the currently selected terrain type.")]
	public ushort selectedTerrainType;

	private void Awake() {
		raycaster = GetComponent<TycoonTileRaycaster>();
	}

	private void Update() {
		if (Input.GetMouseButton(0) && selection.HasSelection) {
			Paint();
		}
	}

	private void OnEnable() {
		raycaster.OnSelectionChangedEvent.AddListener(OnSelectionChanged);
		raycaster.UnlockSelection();
	}

	private void OnDisable() {
		selection.Clear();
		raycaster.OnSelectionChangedEvent.RemoveListener(OnSelectionChanged);
	}

	private void OnSelectionChanged(TerrainSelection newSelection) {
		selection = newSelection;
	}

	private void Paint() {
		selection.Terrain.SchedulePaintOperation(new PaintTerrainBoundsOperation(selection.Bounds, selectedTerrainType));
		selection.Clear();
		Debug.Log("Paint");
	}
}

using TycoonTerrain.Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the tycoon terrain raycaster.
/// This class is responsible for translating camera position and mouse screen position into a terrain selection.
/// </summary>
[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
[AddComponentMenu("Tycoon Tile/Tools/Tycoon Tile Raycaster")]
public class TycoonTileRaycaster : MonoBehaviour {
	/// <summary>
	/// The maximum distance the raycast should reach.
	/// </summary>
	public float MaxRayDistance;

	/// <summary>
	/// The layer mask that the raycast should collide with.
	/// </summary>
	public LayerMask LayerMask = ~0; //Select everything by default

	[Tooltip("Gets called whenever the terrain selection changes")]
	public TerrainSelectionEvent OnSelectionChangedEvent = new TerrainSelectionEvent();

	/// <summary>
	/// The current terrain selection.
	/// </summary>
	private TerrainSelection selection;

	/// <summary>
	/// Cached reference to the camera component from which the raycast is fired.
	/// </summary>
	private Camera cam;

	/// <summary>
	/// Whether the terrain selection is currently locked.
	/// </summary>
	private bool isSelectionLocked;

	/// <summary>
	/// Internal brush size.
	/// </summary>
	private int brushSize;

	public int BrushSize {
		get { return brushSize; }
		set {
			brushSize = value;
			selection.Clear();
			FindNewPosition();
		}
	}

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		selection = new TerrainSelection();
	}

	
	// Update is called once per frame
	void Update() {
		if (!isSelectionLocked) {
			FindNewPosition();
		}
		else if(selection.HasSelection) {
			UpdatePreview();
		}
	}

	public Vector3 WorldToScreenPoint(Vector3 worldPosition) {
		return cam.WorldToScreenPoint(worldPosition);
	}

	private void OnDisable() {
		selection.Clear();
		UpdatePreview();
	}

	/// <summary>
	/// Prevent the selection from changing until it is unlocked again
	/// </summary>
	public void LockSelection() {
		isSelectionLocked = true;
	}

	/// <summary>
	/// Allow the selection to be changed again.
	/// </summary>
	public void UnlockSelection() {
		isSelectionLocked = false;
	}

	/// <summary>
	/// Update the selection by sending a raycast from the mouse position into the scene
	/// </summary>
	private void FindNewPosition () {
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, MaxRayDistance, LayerMask.value)) {
			Vector3 pos = hit.point;
			TycoonTileMap tt = hit.collider.GetComponentInParent<TycoonTileMap>();
			if (tt == null) {
				selection.Clear();
				UpdatePreview();
				return;
			}

			pos = tt.transform.InverseTransformPoint(pos);

			int x = Mathf.RoundToInt(pos.x);
			int z = Mathf.RoundToInt(pos.z);
			int2 selectionPosition = new int2(x, z);

			if (!tt.IsInBounds(selectionPosition)) {
				selection.Clear();
				UpdatePreview();
				return;
			}

			selection.Terrain = tt;

			HandleBrush(tt, pos);
		}
		else if(selection.HasSelection) {
			selection.Clear();
			UpdatePreview();
		}
	}

	/// <summary>
	/// Update the terrain selection with the current mouseWorldPosition
	/// </summary>
	/// <param name="terrain"></param>
	/// <param name="mouseWorldPosition"></param>
	private void HandleBrush(TycoonTileMap terrain, Vector3 mouseWorldPosition) {
		int2 tilePosition = TilePosition.WorldToTilePosition(mouseWorldPosition);
		if (!terrain.IsInBounds(tilePosition)) {
			return;
		}

		if (BrushSize == 0) {
			HandleSingleBrush(terrain, tilePosition, mouseWorldPosition);
		}
		else {
			HandleBrushSize(terrain, tilePosition);
		}
	}

	/// <summary>
	/// Update the terrain selection for brush sizes larger than 0
	/// </summary>
	/// <param name="terrain">The terrain.</param>
	/// <param name="tilePosition">The tile position</param>
	private void HandleBrushSize(TycoonTileMap terrain, int2 tilePosition) {
		if (math.any(tilePosition != selection.Position)) {
			selection.handle = terrain.Grid.GetTile(tilePosition);
			selection.Size = BrushSize;
			selection.CornerIndex = CornerIndex.Invalid;

			UpdatePreview();
		}
	}

	/// <summary>
	/// Update the terrain selection for the individual corner selection size
	/// </summary>
	/// <param name="terrain">The terrain.</param>
	/// <param name="tilePosition">The tile position.</param>
	/// <param name="pos">The mouse position in world space.</param>
	private void HandleSingleBrush(TycoonTileMap terrain, int2 tilePosition, Vector3 pos) {
		TileHandle handle = terrain.Grid.GetTile(tilePosition);
		bool isInCenter = IsMouseInTileCenter(handle, pos);

		CornerIndex index = isInCenter ? CornerIndex.Invalid : GetCornerIndexAtPosition(pos, tilePosition);
		int previousSelectionSize = selection.Size;
		selection.Size = 1;

		if (math.any(tilePosition != selection.Position) || BrushSize != previousSelectionSize || index != selection.CornerIndex) {
			selection.CornerIndex = index;
			selection.handle = handle;

			UpdatePreview();
		}
	}

	/// <summary>
	/// Find the nearest corner of the tile at <paramref name="tilePosition"/> from <paramref name="position"/>.
	/// </summary>
	/// <param name="position">The world position.</param>
	/// <param name="tilePosition">The tile position to find the nearest corner of.</param>
	/// <returns>The corner index of the nearest corner.</returns>
	private CornerIndex GetCornerIndexAtPosition(Vector3 position, int2 tilePosition) {
		float xLocal = position.x - tilePosition.x;
		float zLocal = position.z - tilePosition.y;
		if (xLocal > 0 && zLocal > 0) {
			return CornerIndex.NorthEast;
		}
		if (xLocal > 0 && zLocal < 0) {
			return CornerIndex.SouthEast;
		}
		if (xLocal < 0 && zLocal < 0) {
			return CornerIndex.SouthWest;
		}
		if (xLocal < 0 && zLocal > 0) {
			return CornerIndex.NorthWest;
		}

		//Use northeast as tiebreaker.
		return CornerIndex.NorthEast;
	}

	/// <summary>
	/// Call preview renderer event.
	/// </summary>
	private void UpdatePreview() {
		OnSelectionChangedEvent.Invoke(selection);
	}

	/// <summary>
	/// Test whether the mouse position in worldspace is currently considered to be in the center of a tile.
	/// </summary>
	/// <param name="handle">The tile handle.</param>
	/// <param name="mousePos">The mouse position in world space.</param>
	/// <returns><c>True</c> when <paramref name="mousePos"/> is in the center of the tile, <c>false</c> otherwise.</returns>
	private bool IsMouseInTileCenter(TileHandle handle, Vector3 mousePos) {
		Vector3 tileCenter = handle.CenterPosition;
		tileCenter = new Vector3(tileCenter.x, 0, tileCenter.z);
		Vector3 pos = new Vector3(mousePos.x, 0, mousePos.z);
		return Vector3.Distance(tileCenter, pos) < 0.20f;
	}
}

public class TerrainSelectionEvent : UnityEvent<TerrainSelection> {}
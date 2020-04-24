using TycoonTerrain;
using UnityEngine;

/// <summary>
/// This is the previewer component for the water tool. It projects a water line at the current selection.
/// </summary>
[RequireComponent(typeof(WaterTool))]
public class WaterToolPreviewer : MonoBehaviour {
	[Tooltip("The material to use to render the projector")]
	public Material ProjectorMaterial;

	/// <summary>
	/// Reference to water tool.
	/// </summary>
	private WaterTool waterTool;

	/// <summary>
	/// Reference to the projector.
	/// </summary>
	private GameObject projector;

	/// <summary>
	/// Last know raycasthit.
	/// </summary>
	private RaycastHit lastHit;

    // Start is called before the first frame update
    void Start() {
	    waterTool = GetComponent<WaterTool>();
		projector = new GameObject("Watertool Projector");
		var proj = projector.AddComponent<Projector>();
		proj.orthographic = true;
		proj.orthographicSize = 0.5f;
		proj.aspectRatio = 5;
		proj.farClipPlane = 5f;
		proj.material = ProjectorMaterial;
    }

    // Update is called once per frame
    void Update() {
	    bool projectorEnabled = waterTool.GetLastRaycastHit(out lastHit, out float waterHeight);

		projector.SetActive(projectorEnabled);

	    if (!projectorEnabled) {
		    return;
	    }

	    Vector3 pos = lastHit.point;
	    pos.y = waterHeight;

	    Vector3 horizontalNormal = lastHit.normal;
	    horizontalNormal.Scale(new Vector3(1, 0, 1));
	    horizontalNormal.Normalize();
	    Debug.DrawRay(pos, horizontalNormal, Color.blue);

	    projector.transform.position = pos + 2 * horizontalNormal;
		projector.transform.rotation = Quaternion.LookRotation(-horizontalNormal, Vector3.up);
    }
}

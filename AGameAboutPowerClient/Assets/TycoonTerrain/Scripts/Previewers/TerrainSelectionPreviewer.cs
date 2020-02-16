using System.Collections.Generic;
using TycoonTerrain.Core;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain {
	/// <summary>
	/// Renders a preview mesh for the terraforming tool
	/// </summary>
	[RequireComponent(typeof(TycoonTileRaycaster))]
	public class TerrainSelectionPreviewer : MonoBehaviour {
		[Tooltip("Whether rendering of the preview mesh is enabled. Note that when this is disabled, the preview mesh will still be updated on selection change.")]
		public bool ShowPreview;
		[Tooltip("The material to use for rendering the preview mesh")]
		public Material PreviewMaterial;

		/// <summary>
		/// Reference to the raycaster
		/// </summary>
		private TycoonTileRaycaster raycaster;

		/// <summary>
		/// Reference to the terrain component transform
		/// </summary>
		private Transform targetTerrainTransform;

		private TerrainSelection currentSelection;

		/// <summary>
		/// Temporary mesh data
		/// </summary>
		private List<Vector3> vertices;
		private List<Vector2> uvs;
		private List<int> indices;

		/// <summary>
		/// Reference to the internal mesh that will be drawn
		/// </summary>
		private Mesh mesh;

		private readonly Vector2[] unitUvs = new[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1)};

		public void Awake() {
			raycaster = GetComponent<TycoonTileRaycaster>();
			mesh = new Mesh();
			int vertexCount = 3 * 4 * 4;
			vertices = new List<Vector3>(vertexCount);
			uvs = new List<Vector2>(vertexCount);
			indices = new List<int>(16);
		}

		public void OnEnable() {
			raycaster.OnSelectionChangedEvent.AddListener(OnSelectionChanged);
		}

		public void OnDisable() {
			raycaster.OnSelectionChangedEvent.RemoveListener(OnSelectionChanged);
			mesh.Clear(false);
		}

		public void Update() {
			if(targetTerrainTransform != null)
				Graphics.DrawMesh(mesh, targetTerrainTransform.localToWorldMatrix, PreviewMaterial, gameObject.layer);
		}

		public void OnSelectionChanged(TerrainSelection selection) {
			vertices.Clear();
			uvs.Clear();
			indices.Clear();
			mesh.Clear(false);

			if (!selection.HasSelection || !ShowPreview) {
				return;
			}

			TileHandle handle = selection.handle.Value;
			TerrainGrid terrain = selection.Terrain.Grid;

			targetTerrainTransform = selection.Terrain.transform;
			currentSelection = selection;

			//Find the legal bounds within the terrain
			IntBound realBound = terrain.IntersectBound(selection.Bounds);
			int2 min = realBound.Min;
			int2 max = realBound.Max;


			if (selection.IsCenter) {
				ShowPreviewAt(terrain.GetTile(max.x, max.y), CornerIndex.NorthEast);
				ShowPreviewAt(terrain.GetTile(max.x, min.y), CornerIndex.SouthEast);
				ShowPreviewAt(terrain.GetTile(min.x, min.y), CornerIndex.SouthWest);
				ShowPreviewAt(terrain.GetTile(min.x, max.y), CornerIndex.NorthWest);
			}
			else {
				ShowPreviewAt(handle, selection.CornerIndex);
			}

			mesh.SetVertices(vertices);
			mesh.SetUVs(0, uvs);
			mesh.SetTriangles(indices, 0);
			mesh.RecalculateNormals();
		}

		private float3 GetCornerPosition(ref TileHandle handle, CornerIndex index) {
			return TileHandle.cornerOffsets[index.Index] + handle.CenterPosition + Vector3.up * 0.0025f + new Vector3(0, handle.GetData().GetHeight(index) * currentSelection.Terrain.WorldHeightStep, 0);
		}

		private void ShowPreviewAt(TileHandle handle, CornerIndex cornerIndex) {
			int i = vertices.Count;

			float3 ne = GetCornerPosition(ref handle, CornerIndex.NorthEast);
			float3 se = GetCornerPosition(ref handle, CornerIndex.SouthEast);
			float3 sw = GetCornerPosition(ref handle, CornerIndex.SouthWest);
			float3 nw = GetCornerPosition(ref handle, CornerIndex.NorthWest);
			float3 center = handle.CenterPosition + new Vector3(0, handle.CenterSurfaceHeight * currentSelection.Terrain.WorldHeightStep, 0) + new Vector3(0, + 0.0025f, 0);

			//Rotate uvs based on starting corner index
			Vector2 uvNE = unitUvs[cornerIndex.Index];
			Vector2 uvSE = unitUvs[cornerIndex.NeighbourCounterClockwise.Index];
			Vector2 uvSW = unitUvs[cornerIndex.NeighbourOpposite.Index];
			Vector2 uvNW = unitUvs[cornerIndex.NeighbourClockwise.Index];
			Vector2 uvC = new Vector2(0.5f, 0.5f);

			//North triangle
			vertices.Add(ne);
			vertices.Add(center);
			vertices.Add(nw);

			uvs.Add(uvNE);
			uvs.Add(uvC);
			uvs.Add(uvNW);

			//East Triangle
			vertices.Add(ne);
			vertices.Add(se);
			vertices.Add(center);

			uvs.Add(uvNE);
			uvs.Add(uvSE);
			uvs.Add(uvC);

			//South Triangle
			vertices.Add(se);
			vertices.Add(sw);
			vertices.Add(center);

			uvs.Add(uvSE);
			uvs.Add(uvSW);
			uvs.Add(uvC);

			//West Triangle
			vertices.Add(nw);
			vertices.Add(center);
			vertices.Add(sw);

			uvs.Add(uvNW);
			uvs.Add(uvC);
			uvs.Add(uvSW);

			indices.Add(i);
			indices.Add(i + 1);
			indices.Add(i + 2);

			indices.Add(i + 3);
			indices.Add(i + 4);
			indices.Add(i + 5);

			indices.Add(i + 6);
			indices.Add(i + 7);
			indices.Add(i + 8);

			indices.Add(i + 9);
			indices.Add(i + 10);
			indices.Add(i + 11);
		}
	}
}
using System.Collections.Generic;
using TycoonTerrain.Core;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TycoonTerrain {
	/// <summary>
	/// Renders a preview mesh for the painter tool.
	/// </summary>
	[RequireComponent(typeof(TycoonTileRaycaster))]
	public class TerrainPainterPreviewer : MonoBehaviour {
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

		/// <summary>
		/// Temporary mesh data
		/// </summary>
		private List<Vector3> vertices;
		private List<int> indices;
		private List<Vector2> uvs;

		/// <summary>
		/// Reference to the internal mesh that will be drawn
		/// </summary>
		private Mesh previewMesh;

		public void Awake() {
			raycaster = GetComponent<TycoonTileRaycaster>();
			previewMesh = new Mesh();
			vertices = new List<Vector3>();
			indices = new List<int>();
			uvs = new List<Vector2>();
		}

		public void Update() {
			if(targetTerrainTransform != null)
				Graphics.DrawMesh(previewMesh, targetTerrainTransform.localToWorldMatrix, PreviewMaterial, gameObject.layer);
		}

		public void OnEnable() {
			raycaster.OnSelectionChangedEvent.AddListener(OnSelectionChanged);
		}

		public void OnDisable() {
			raycaster.OnSelectionChangedEvent.RemoveListener(OnSelectionChanged);
		}

		public void OnSelectionChanged(TerrainSelection selection) {
			previewMesh.Clear();

			if (!selection.HasSelection || !ShowPreview) {
				return;
			}

			TerrainGrid terrain = selection.Terrain.Grid;

			targetTerrainTransform = selection.Terrain.transform;

			//Find the legal bounds within the terrain
			IntBound realBound = terrain.IntersectBound(selection.Bounds);

			vertices.Clear();
			indices.Clear();
			uvs.Clear();

			RenderTerrainSurfaceChunkJob renderSurfaceJob = new RenderTerrainSurfaceChunkJob(realBound, terrain, selection.Terrain.TypeTable, selection.Terrain.WorldHeightStep, new NativeMeshData(Allocator.TempJob));

			JobHandle jobHandle = renderSurfaceJob.Schedule();

			jobHandle.Complete();

			NativeMeshData.CopyData(renderSurfaceJob.NativeMeshData, vertices, indices, uvs);

			for (int i = 0; i < vertices.Count; i++) {
				vertices[i] = vertices[i] + new Vector3(0, 0.0025f, 0);
			}

			previewMesh.SetVertices(vertices);
			previewMesh.SetTriangles(indices, 0);

			renderSurfaceJob.Dispose();
			renderSurfaceJob.NativeMeshData.Dispose();

		}
	}
}
using System.Collections;
using System.Collections.Generic;
using Assets.TycoonTerrain.Scripts.Core;
using TycoonTerrain.Core;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

/// <summary>
/// The tycoon terrain renderer is responsible for updating meshes based on the tile data.
/// </summary>
[AddComponentMenu("Tycoon Tile/Tycoon Tile Renderer")]
[DisallowMultipleComponent]
[RequireComponent(typeof(TycoonTileMap))]
public class TycoonTileRenderer : MonoBehaviour, IChunkListener {
	public TerrainTypeCollectionAsset TerrainTypes;

	public ShadowCastingMode CastShadows = ShadowCastingMode.Off;
	public bool ReceiveShadows = false;

	//Temporary mesh data.
	List<Vector3> verts = new List<Vector3>();
	List<List<int>> tris = new List<List<int>>();
	List<Vector2> uvs = new List<Vector2>();

	/// <summary>
	/// Holds references to the actual Mesh instances associated with a given chunk.
	/// </summary>
	private Dictionary<int2, ChunkMesh> meshCache;

	/// <summary>
	/// Holds a reference to the terrain component.
	/// </summary>
	private TycoonTileMap terrain;

	void Awake() {
		terrain = GetComponent<TycoonTileMap>();
		if (terrain == null) {
			Debug.LogWarning("TycoonTerrainRenderer component needs to have a TycoonTerrainComponent attached to the same object. Please attach a TycoonTerrainComponent.");
			enabled = false;
		}
		meshCache = new Dictionary<int2, ChunkMesh>();
	}

	// Use this for initialization
	void Start () {
		if (TerrainTypes == null) {
			Debug.LogWarning("There is no terrain types collection assigned to the tycoon tile renderer. Please assign a valid terrain types collection.");
			enabled = false;
			return;
		}

		if (!TerrainTypes.HasDefinitions) {
			Debug.LogWarning("The terrain types collection " + TerrainTypes.name + " has no terrain types assigned to its definition list. Please add at least one terrain type definition to the collection.");
			enabled = false;
			return;
		}

		terrain.RegisterDirtyChunkListener(this);
	}

	/// <summary>
	/// Draw all chunk meshes procedurally every frame.
	/// </summary>
	private void LateUpdate() {
		foreach (ChunkMesh chunkMesh in meshCache.Values) {
			Vector3 chunkWorldPosition = transform.position;
			Quaternion chunkWorldRotation = transform.rotation;

			for (int i = 0; i < chunkMesh.SurfaceMesh.subMeshCount; i++) {
				Graphics.DrawMesh(chunkMesh.SurfaceMesh, chunkWorldPosition, chunkWorldRotation, chunkMesh.GetSurfaceMaterial(TerrainTypes, i), gameObject.layer, null, i, null, CastShadows, ReceiveShadows);
			}

			for (int i = 0; i < chunkMesh.CliffMesh.subMeshCount; i++) {
				Graphics.DrawMesh(chunkMesh.CliffMesh, chunkWorldPosition, chunkWorldRotation, chunkMesh.GetCliffMaterial(TerrainTypes, i), gameObject.layer, null, i, null, CastShadows, ReceiveShadows);
			}

			Graphics.DrawMesh(chunkMesh.WaterMesh, chunkWorldPosition, chunkWorldRotation, TerrainTypes.WaterMaterial, gameObject.layer, null, 0, null, ShadowCastingMode.Off, false);
			Graphics.DrawMesh(chunkMesh.WaterCliffMesh, chunkWorldPosition, chunkWorldRotation, TerrainTypes.WaterCliffMaterial, gameObject.layer, null, 0, null, ShadowCastingMode.Off, false);
		}
	}

	public void GenerateMesh(Transform parent)
	{
		foreach (ChunkMesh chunkMesh in meshCache.Values)
		{
			Vector3 chunkWorldPosition = transform.position;
			Quaternion chunkWorldRotation = transform.rotation;

			for (int i = 0; i < chunkMesh.SurfaceMesh.subMeshCount; i++)
			{
                CreateMeshObject(chunkMesh.SurfaceMesh, chunkWorldPosition, chunkWorldRotation, chunkMesh.GetSurfaceMaterial(TerrainTypes, i), parent, gameObject.layer);
			}

			for (int i = 0; i < chunkMesh.CliffMesh.subMeshCount; i++)
			{
                CreateMeshObject(chunkMesh.CliffMesh, chunkWorldPosition, chunkWorldRotation, chunkMesh.GetCliffMaterial(TerrainTypes, i), parent, gameObject.layer);
			}

            CreateMeshObject(chunkMesh.WaterMesh, chunkWorldPosition, chunkWorldRotation, TerrainTypes.WaterMaterial, parent, gameObject.layer);
            CreateMeshObject(chunkMesh.WaterCliffMesh, chunkWorldPosition, chunkWorldRotation, TerrainTypes.WaterCliffMaterial, parent, gameObject.layer);
		}
	}

    public void CreateMeshObject(Mesh _mesh, Vector3 pos, Quaternion rot, Material mat, Transform _parent, LayerMask layer)
	{
		GameObject obj = new GameObject(_mesh.name);
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		MeshFilter mfilter = obj.AddComponent<MeshFilter>();
		MeshRenderer mrend = obj.AddComponent<MeshRenderer>();
		mfilter.mesh = _mesh;
		mrend.material = mat;
        obj.layer = layer;
        obj.transform.SetParent(_parent);
	}


	/// <summary>
	/// Gets called after completing all operations for this frame. Meshes should be updated in this scope.
	/// </summary>
	/// <param name="terrain">The terrain</param>
	/// <param name="chunksToUpdate">The dirty chunks.</param>
	public void OnUpdateChunks(TycoonTileMap terrain, ref ChunkSet chunksToUpdate) {
		List<ScheduledMeshJobPair> pairs = new List<ScheduledMeshJobPair>();

		Profiler.BeginSample("Scheduling all jobs");
		foreach (int2 chunkPosition in chunksToUpdate.GetChunkPositions()) {
			var pair = ScheduleMeshJobs(terrain, chunkPosition, chunksToUpdate.GetChunkBounds(chunkPosition));
			pairs.Add(pair);
		}
		Profiler.EndSample();

		JobHandle.ScheduleBatchedJobs();

		foreach (var pair in pairs) {
			var chunkPosition = pair.ChunkPosition;
			if (!meshCache.ContainsKey(chunkPosition)) {
				meshCache.Add(chunkPosition, new ChunkMesh(chunkPosition));
			}

			var chunkMesh = meshCache[chunkPosition];

			pair.Complete();
			RenderMesh(pair, chunkMesh);
		}
	}

	private ScheduledMeshJobPair ScheduleMeshJobs(TycoonTileMap terrain, int2 chunkPosition, IntBound bounds) {
		return new ScheduledMeshJobPair(bounds, chunkPosition, terrain);
	}

	private void RenderMesh(ScheduledMeshJobPair pair, ChunkMesh chunkMesh) {
		Profiler.BeginSample("Copy mesh results");

		var surfaceMapping = new Dictionary<ushort, int>();
		var cliffMapping = new Dictionary<ushort, int>();

		CopyDataToMesh(pair.SurfaceMeshData, chunkMesh.SurfaceMesh, surfaceMapping);

		CopyDataToMesh(pair.CliffMeshData, chunkMesh.CliffMesh, cliffMapping);

		CopyDataToMesh(pair.WaterMeshData, chunkMesh.WaterMesh);

		CopyDataToMesh(pair.WaterCliffMeshData, chunkMesh.WaterCliffMesh);

		pair.Dispose();

		chunkMesh.SetTerrainTypeMapping(surfaceMapping, cliffMapping);

		Profiler.EndSample();
	}

	/// <summary>
	/// Copies native mesh data, that was used in the background jobs, to the actual Mesh object.
	/// </summary>
	/// <param name="nativeMeshData">The native mesh data.</param>
	/// <param name="mesh">The mesh.</param>
	/// <param name="mapping">A mapping that maps the triangle terrain type data to submeshes.</param>
	private void CopyDataToMesh(NativeMeshData nativeMeshData, Mesh mesh, Dictionary<ushort, int> mapping = null) {
		for (int i = 0; i < tris.Count; i++) {
			tris.Clear();
		}

		if (mapping == null) {
			if (tris.Count < 1) {
				tris.Add(new List<int>());
			}
			NativeMeshData.CopyData(nativeMeshData, verts, tris[0], uvs);
		}
		else {
			NativeMeshData.CopyData(nativeMeshData, verts, tris, mapping, uvs);
		}

		Profiler.BeginSample("Apply Mesh");
		mesh.Clear(false);
		mesh.indexFormat = IndexFormat.UInt32;
		mesh.SetVertices(verts);
		mesh.SetUVs(0, uvs);
		mesh.subMeshCount = tris.Count;
		for (int i = 0; i < tris.Count; i++) {
			mesh.SetTriangles(tris[i], i);
		}

		mesh.RecalculateNormals();
		Profiler.EndSample();
	}

	private struct ScheduledMeshJobPair {
		public readonly RenderWaterChunkJob waterJob;
		public readonly RenderWaterCliffChunkJob waterCliffJob;
		public readonly RenderTerrainCliffChunkJob cliffJob;
		public readonly RenderTerrainSurfaceChunkJob surfaceJob;

		public JobHandle JobHandle;

		public NativeMeshData SurfaceMeshData => surfaceJob.NativeMeshData;

		public NativeMeshData WaterMeshData => waterJob.NativeMeshData;

		public NativeMeshData CliffMeshData => cliffJob.NativeMeshData;

		public NativeMeshData WaterCliffMeshData => waterCliffJob.NativeMeshData;

		public readonly int2 ChunkPosition;

		/// <summary>
		/// Schedules the jobs necessary to update all meshes.
		/// </summary>
		/// <param name="bounds">The bounds of the chunk to update meshes for.</param>
		/// <param name="chunkPosition">The position of the chunk to update meshes for (in chunk space).</param>
		/// <param name="terrain">The terrain.</param>
		public ScheduledMeshJobPair(IntBound bounds, int2 chunkPosition, TycoonTileMap terrain) {
			surfaceJob = new RenderTerrainSurfaceChunkJob(bounds, terrain.Grid, terrain.TypeTable, terrain.WorldHeightStep, new NativeMeshData(Allocator.Persistent));
			var jobHandle = surfaceJob.Schedule();

			cliffJob = new RenderTerrainCliffChunkJob(bounds, terrain.Grid, terrain.TypeTable, terrain.WorldHeightStep, new NativeMeshData(Allocator.Persistent));
			var cliffJobHandle = cliffJob.Schedule();

			waterJob = new RenderWaterChunkJob(bounds, terrain.Grid, new NativeMeshData(Allocator.Persistent), terrain.WaterHeightStep);
			var waterJobHandle = waterJob.Schedule();

			waterCliffJob = new RenderWaterCliffChunkJob(bounds, terrain.Grid, new NativeMeshData(Allocator.Persistent), terrain.WorldHeightStep, terrain.WaterHeightStep);
			var waterCliffJobHandle = waterCliffJob.Schedule();

			NativeArray<JobHandle> handles = new NativeArray<JobHandle>(4, Allocator.Temp);
			handles[0] = jobHandle;
			handles[1] = cliffJobHandle;
			handles[2] = waterJobHandle;
			handles[3] = waterCliffJobHandle;

			JobHandle = JobHandle.CombineDependencies(handles);
			handles.Dispose();
			ChunkPosition = chunkPosition;
		}

		public void Complete() {
			JobHandle.Complete();
		}

		public void Dispose() {
			waterJob.NativeMeshData.Dispose();
			waterCliffJob.NativeMeshData.Dispose();
			cliffJob.NativeMeshData.Dispose();
			surfaceJob.NativeMeshData.Dispose();
			surfaceJob.Dispose();
		}
	}
}

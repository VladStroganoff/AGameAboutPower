using System.Collections;
using System.Collections.Generic;
using TycoonTerrain.Core;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[AddComponentMenu("Tycoon Tile/Tycoon Tile Collider")]
[DisallowMultipleComponent]
[RequireComponent(typeof(TycoonTileMap))]
public class TycoonTileCollider : MonoBehaviour, IChunkListener {
	private TycoonTileMap terrain;
	private Dictionary<int2, MeshCollider> colliders;

	// Use this for initialization
	void Awake () {
		terrain = GetComponent<TycoonTileMap>();
		colliders = new Dictionary<int2, MeshCollider>();
	}

	void Start() {
		int chunksX = Mathf.CeilToInt(terrain.Width / (float)terrain.ChunkSize);
		int chunksZ = Mathf.CeilToInt(terrain.Length / (float)terrain.ChunkSize);

		for (int x = 0; x < chunksX; x++) {
			for (int z = 0; z < chunksZ; z++) {
				GameObject obj = new GameObject($"Collider ({x}, {z})");
				obj.transform.SetParent(terrain.transform, false);
				obj.layer = gameObject.layer;

				MeshCollider meshCollider = obj.AddComponent<MeshCollider>();

				colliders.Add(new int2(x, z), meshCollider);
			}
		}

		terrain.RegisterDirtyChunkListener(this);
	}

	public void SetCollider(int2 chunk, Mesh mesh) {
		if (colliders.TryGetValue(chunk, out MeshCollider chunkCollider)) {
			chunkCollider.sharedMesh = mesh;
		}
		else {
			Debug.LogError("Collider not found for chunk " + chunk);
		}
	}

	public Mesh GenerateCollider(int2 chunk) {
		var bounds = terrain.Bounds.Intersection(new IntBound(chunk * terrain.ChunkSize, (chunk + new int2(1)) * terrain.ChunkSize));
		var job = new RenderTerrainSurfaceChunkJob(bounds, terrain.Grid, terrain.TypeTable, terrain.WorldHeightStep, new NativeMeshData(Allocator.Persistent));

		var surfaceHandle = job.Schedule();

		var cliffJob = new RenderTerrainCliffChunkJob(bounds, terrain.Grid, terrain.TypeTable, terrain.WorldHeightStep, new NativeMeshData(Allocator.Persistent));

		var cliffHandle = cliffJob.Schedule();

		var handle = JobHandle.CombineDependencies(surfaceHandle, cliffHandle);
		handle.Complete();

		var verts = new List<Vector3>();
		var tris = new List<int>();
		var uvs = new List<Vector2>();

		NativeMeshData.CopyData(job.NativeMeshData, verts, tris, uvs);

		var surfaceMesh = new Mesh();
		surfaceMesh.SetVertices(verts);
		surfaceMesh.SetTriangles(tris, 0);

		verts.Clear();
		tris.Clear();
		uvs.Clear();

		NativeMeshData.CopyData(cliffJob.NativeMeshData, verts, tris, uvs);

		var cliffMesh = new Mesh();
		cliffMesh.SetVertices(verts);
		cliffMesh.SetTriangles(tris, 0);

		var colliderMesh = new Mesh();
		var combine = new CombineInstance[] {
			new CombineInstance {mesh = surfaceMesh, subMeshIndex = 0}, 
			new CombineInstance {mesh = cliffMesh, subMeshIndex = 0}
		};

		colliderMesh.CombineMeshes(combine, true, false, false);


		job.Dispose();
		job.NativeMeshData.Dispose();
		cliffJob.NativeMeshData.Dispose();

		return colliderMesh;
	}

	public void OnUpdateChunks(TycoonTileMap terrain, ref ChunkSet chunks) {
		foreach (int2 position in chunks.GetChunkPositions()) {
			Mesh mesh = GenerateCollider(position);

			SetCollider(position, mesh);
		}
	}
}

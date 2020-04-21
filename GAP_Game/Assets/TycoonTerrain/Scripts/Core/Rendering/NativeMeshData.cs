using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace TycoonTerrain.Core {
	public struct NativeMeshData {
		public NativeList<SubMeshTriangle> indices;
		public NativeList<float3> vertices;
		public NativeList<float2> uvs;

		public NativeMeshData(Allocator allocator) {
			indices = new NativeList<SubMeshTriangle>(allocator);
			vertices = new NativeList<float3>(allocator);
			uvs = new NativeList<float2>(allocator);
		}

		public NativeMeshData(NativeList<SubMeshTriangle> indices, NativeList<float3> verts, NativeList<float2> uv) {
			this.indices = indices;
			vertices = verts;
			uvs = uv;
		}

		public void Dispose() {
			indices.Dispose();
			vertices.Dispose();
			uvs.Dispose();
		}

		public static void CopyData(NativeMeshData from, List<Vector3> vertices, List<List<int>> indices, Dictionary<ushort, int> terrainTypeMapping, List<Vector2> uvs) {
			var nativeVertices = from.vertices.AsArray();
			var nativeUvs = from.uvs.AsArray();

			vertices.Clear();
			uvs.Clear();

			if (vertices.Capacity < nativeVertices.Length) {
				vertices.Capacity = nativeVertices.Length;
			}
			
			if (uvs.Capacity < nativeUvs.Length) {
				uvs.Capacity = nativeUvs.Length;
			}

			Profiler.BeginSample("Copy arrays");
			CopyIndices(from, indices, terrainTypeMapping);

			for (int i = 0; i < nativeVertices.Length; i++) {
				float3 vertex = nativeVertices[i];
				vertices.Add(vertex);
			}

			for (int i = 0; i < nativeUvs.Length; i++) {
				float2 uv = nativeUvs[i];
				uvs.Add(uv);
			}
			Profiler.EndSample();
		}

		public static void CopyData(NativeMeshData from, List<Vector3> vertices, List<int> indices, List<Vector2> uvs) {
			var nativeVertices = from.vertices.AsArray();
			var nativeUvs = from.uvs.AsArray();

			vertices.Clear();
			uvs.Clear();

			if (vertices.Capacity < nativeVertices.Length) {
				vertices.Capacity = nativeVertices.Length;
			}
			
			if (uvs.Capacity < nativeUvs.Length) {
				uvs.Capacity = nativeUvs.Length;
			}

			Profiler.BeginSample("Copy arrays");
			CopyIndicesFlat(from, indices);

			for (int i = 0; i < nativeVertices.Length; i++) {
				float3 vertex = nativeVertices[i];
				vertices.Add(vertex);
			}

			for (int i = 0; i < nativeUvs.Length; i++) {
				float2 uv = nativeUvs[i];
				uvs.Add(uv);
			}
			Profiler.EndSample();
		}

		/// <summary>
		/// Ignores terrain type info and copies the indices to a flat array
		/// </summary>
		/// <param name="from">The source native mesh data.</param>
		/// <param name="indices">The destination indices list.</param>
		public static void CopyIndicesFlat(NativeMeshData from, List<int> indices) {
			var nativeIndices = from.indices.AsArray();
			indices.Clear();
			if (indices.Capacity < nativeIndices.Length) {
				indices.Capacity = nativeIndices.Length;
			}

			for (int i = 0; i < nativeIndices.Length; i++) {
				SubMeshTriangle ind = nativeIndices[i];
				indices.Add(ind.TriangleIndex);
			}
		}

		public static void CopyIndices(NativeMeshData from, List<List<int>> indices, Dictionary<ushort, int> mapping) {
			var nativeIndices = from.indices.AsArray();
			indices.Clear();
			if (indices.Capacity < nativeIndices.Length) {
				indices.Capacity = nativeIndices.Length;
			}

			for (int i = 0; i < nativeIndices.Length; i++) {
				SubMeshTriangle ind = nativeIndices[i];

				if (!mapping.TryGetValue(ind.TerrainType, out int listIndex)) {
					int newIndex = indices.Count;
					mapping.Add(ind.TerrainType, newIndex);
					listIndex = newIndex;
					indices.Add(new List<int>());
				}
				
				indices[listIndex].Add(ind.TriangleIndex);
			}
		}
	}
}
using System.Collections.Generic;
using Assets.TycoonTerrain.Scripts.Core;
using Unity.Mathematics;
using UnityEngine;

namespace TycoonTerrain.Core {
	public class ChunkMesh {

		public Mesh SurfaceMesh;
		public Mesh CliffMesh;
		public Mesh WaterMesh;
		public Mesh WaterCliffMesh;

		private Dictionary<int, ushort> surfaceSubMeshTerrainTypes;
		private Dictionary<int, ushort> cliffSubMeshTerrainTypes;

		public ChunkMesh(int2 position) {
			SurfaceMesh = new Mesh();
			CliffMesh = new Mesh();
			WaterMesh = new Mesh();
			WaterCliffMesh = new Mesh();

			surfaceSubMeshTerrainTypes = new Dictionary<int, ushort>();
			cliffSubMeshTerrainTypes = new Dictionary<int, ushort>();
			surfaceSubMeshTerrainTypes.Add(0, 0);
			cliffSubMeshTerrainTypes.Add(0, 0);

#if UNITY_EDITOR
			SurfaceMesh.name = "Tycoon Terrain Surface Mesh " + position;
			CliffMesh.name = "Tycoon Terrain Cliff Mesh " + position;
			WaterMesh.name = "Tycoon Terrain Water Mesh " + position;
			WaterCliffMesh.name = "Tycoon Terrain Water Cliff Mesh " + position;
#endif
		}

		public Material GetSurfaceMaterial(TerrainTypeCollectionAsset terrainTypeCollection, int i) {
			var type = surfaceSubMeshTerrainTypes[i];
			return terrainTypeCollection.GetSurfaceMaterialForTerrainType(type);
		}

		public Material GetCliffMaterial(TerrainTypeCollectionAsset terrainTypeCollection, int i) {
			var type = cliffSubMeshTerrainTypes[i];
			return terrainTypeCollection.GetCliffMaterialForTerrainType(type);
		}

		public void SetTerrainTypeMapping(Dictionary<ushort, int> surfaceMapping, Dictionary<ushort, int> cliffMapping) {
			surfaceSubMeshTerrainTypes.Clear();
			cliffSubMeshTerrainTypes.Clear();

			foreach (KeyValuePair<ushort, int> pair in surfaceMapping) {
				surfaceSubMeshTerrainTypes.Add(pair.Value, pair.Key);
			}

			foreach (KeyValuePair<ushort, int> pair in cliffMapping) {
				cliffSubMeshTerrainTypes.Add(pair.Value, pair.Key);
			}
		}
	}
}
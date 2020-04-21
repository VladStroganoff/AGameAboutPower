namespace TycoonTerrain.Core {
	public struct SubMeshTriangle {
		public ushort TerrainType { get; }

		public int TriangleIndex { get; }

		public SubMeshTriangle(ushort terrainType, int triangleIndex) {
			TerrainType = terrainType;
			TriangleIndex = triangleIndex;
		}
	}
}
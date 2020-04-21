using UnityEngine;

namespace TycoonTerrain.Core {
	[CreateAssetMenu(menuName = "Tycoon Tile/Terrain Type Definition", order = 300)]
	public class TerrainTypeDefinition : ScriptableObject {
		[Tooltip("Used for rendering the surface of tiles with this terrain type.")]
		public Material SurfaceMaterial;
		[Tooltip("Used for rendering the cliff of tiles with this terrain type.")]
		public Material CliffMaterial;
	}
}
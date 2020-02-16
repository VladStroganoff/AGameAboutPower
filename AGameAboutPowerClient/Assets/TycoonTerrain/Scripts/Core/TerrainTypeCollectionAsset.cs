using TycoonTerrain.Core;
using UnityEngine;

namespace Assets.TycoonTerrain.Scripts.Core {
	/// <summary>
	/// This asset is responsible for containing all terrain type definitions and their respective materials.
	/// </summary>
	[CreateAssetMenu(menuName = "Tycoon Tile/Create Terrain Type Collection", order = 300)]
	public class TerrainTypeCollectionAsset : ScriptableObject {
		[Tooltip("The list of terrain type definitions. You must include at least one definition.")]
		public TerrainTypeDefinition[] Definitions;
		[Tooltip("The material that should be used to render water surfaces.")]
		public Material WaterMaterial;
		[Tooltip("The material that should be used to render the cliffs of water tiles.")]
		public Material WaterCliffMaterial;

		/// <summary>
		/// Gets whether this asset includes any valid terrain type definitions.
		/// </summary>
		public bool HasDefinitions {
			get {
				if (Definitions == null || Definitions.Length == 0)
					return false;

				//Check all definition entries
				for (int i = 0; i < Definitions.Length; i++) {
					if (Definitions[i] != null)
						return true;
				}

				//All definitions in the array were null
				return false;
			}
		}

		/// <summary>
		/// Gets the material that should be used to render the surface for a specific terrain type.
		/// </summary>
		/// <param name="type">The terrain type.</param>
		public Material GetSurfaceMaterialForTerrainType(ushort type) {
			if (type > Definitions.Length)
				return null;

			return Definitions[type].SurfaceMaterial;
		}

		/// <summary>
		/// Gets the material that should be used to render the cliff for a specific terrain type.
		/// </summary>
		/// <param name="type">The terrain type.</param>
		public Material GetCliffMaterialForTerrainType(ushort type) {
			if (type > Definitions.Length)
				return null;

			return Definitions[type].CliffMaterial;
		}
	}
}

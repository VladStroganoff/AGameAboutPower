using NUnit.Framework;
using TycoonTerrain.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Tests
{
	public class TileHandleTests
	{
		// A Test behaves as an ordinary method
		[Test]
		public void IsFlushWithFlat() {
			TerrainGrid grid = new TerrainGrid(4, 4);
			grid.ResetData();

			TileHandle origin = grid.GetTile(0, 0);

			Assert.That(origin.IsEdgeFlush(CardinalDirection.North));
		}

        
	}
}
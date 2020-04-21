using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TycoonTerrain.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CornerIndexTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CornerIndexTestCornerOfDirection() {
			//Test north left and right
	        var corner = CornerIndex.GetCornerOfDirection(CardinalDirection.North, true);

			Assert.That(corner == CornerIndex.NorthWest);

			corner = CornerIndex.GetCornerOfDirection(CardinalDirection.North, false);

			Assert.That(corner == CornerIndex.NorthEast);


			//Test South Left and Right
			corner = CornerIndex.GetCornerOfDirection(CardinalDirection.South, true);

			Assert.That(corner == CornerIndex.SouthEast);

			corner = CornerIndex.GetCornerOfDirection(CardinalDirection.South, false);

			Assert.That(corner == CornerIndex.SouthWest);
        }

        
    }
}

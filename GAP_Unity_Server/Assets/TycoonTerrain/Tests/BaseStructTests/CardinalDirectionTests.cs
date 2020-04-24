using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TycoonTerrain.Core;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CardinalDirectionTests
    {
        [Test]
        public void CardinalDirectionTestsInverseDirections()
        {
            Assert.That(CardinalDirection.North.Inverse == CardinalDirection.South);

            Assert.That(CardinalDirection.East.Inverse == CardinalDirection.West);

            Assert.That(CardinalDirection.South.Inverse == CardinalDirection.North);

            Assert.That(CardinalDirection.West.Inverse == CardinalDirection.East);
        }

        [Test]
        public void DirectionRotateLeftTests() {
	        Assert.That(CardinalDirection.North.RotateLeft == CardinalDirection.West);
	        Assert.That(CardinalDirection.East.RotateLeft == CardinalDirection.North);
	        Assert.That(CardinalDirection.South.RotateLeft == CardinalDirection.East);
	        Assert.That(CardinalDirection.West.RotateLeft == CardinalDirection.South);
        }

        [Test]
        public void DirectionRotateRightTests() {
	        Assert.That(CardinalDirection.North.RotateRight == CardinalDirection.East);
	        Assert.That(CardinalDirection.East.RotateRight == CardinalDirection.South);
	        Assert.That(CardinalDirection.South.RotateRight == CardinalDirection.West);
	        Assert.That(CardinalDirection.West.RotateRight == CardinalDirection.North);
        }

        [Test]
        public void GetDirectionFromCornerTests() {
	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.NorthEast, true) == CardinalDirection.North);
	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.NorthEast, false) == CardinalDirection.East);

	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.SouthEast, true) == CardinalDirection.East);
	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.SouthEast, false) == CardinalDirection.South);

	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.SouthWest, true) == CardinalDirection.South);
	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.SouthWest, false) == CardinalDirection.West);

	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.NorthWest, true) == CardinalDirection.West);
	        Assert.That(CardinalDirection.GetDirectionFromCorner(CornerIndex.NorthWest, false) == CardinalDirection.North);
        }
    }
}

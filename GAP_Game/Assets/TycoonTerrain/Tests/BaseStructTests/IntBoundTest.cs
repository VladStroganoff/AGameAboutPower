using NUnit.Framework;
using TycoonTerrain.Core;
using Unity.Mathematics;

namespace Tests
{
    public class IntBoundTest
    {
        [Test]
        public void IntBoundManhattenDistance()
        {
			IntBound bound = new IntBound(new int2(5,5), new int2(7,7));

			//Check that the distance for points in the bounds is 0
			Assert.That(bound.Distance(new int2(6, 6)) == 0);

			Assert.That(bound.Distance(new int2(4, 5)) == 1);

			Assert.That(bound.Distance(new int2(5, 3)) == 2);

			Assert.That(bound.Distance(new int2(4, 4)) == 2);

			Assert.That(bound.Distance(new int2(8, 7)) == 1);

			Assert.That(bound.Distance(new int2(7, 9)) == 2);

			Assert.That(bound.Distance(new int2(8, 8)) == 2);
        }

        [Test]
        public void IntBoundContains()
        {
	        IntBound bound = new IntBound(new int2(5,5), new int2(7,7));

	        //Test points inside the bound
	        Assert.That(bound.Contains(new int2(6, 6)));

	        Assert.That(bound.Contains(new int2(5, 5)));

	        Assert.That(bound.Contains(new int2(5, 6)));

	        Assert.That(bound.Contains(new int2(7, 7)));

	        Assert.That(bound.Contains(new int2(6, 7)));

			//Test points that are outside the bound
	        Assert.That(!bound.Contains(new int2(1, 1)));

	        Assert.That(!bound.Contains(new int2(1, 6)));

	        Assert.That(!bound.Contains(new int2(6, 1)));

	        Assert.That(!bound.Contains(new int2(9, 6)));

	        Assert.That(!bound.Contains(new int2(6, 9)));

	        Assert.That(!bound.Contains(new int2(9, 9)));
        }

        [Test]
        public void IntBoundExpandDirection() {
	        int2 min = new int2(5, 5);
	        int2 max = new int2(7, 7);
	        IntBound bound = new IntBound(min, max);

	        //Test points inside the bound
	        Assert.That(bound.Expand(CardinalDirection.North, 2) == new IntBound(min, new int2(7, 9)));
			
	        Assert.That(bound.Expand(CardinalDirection.South, 2) == new IntBound(new int2(5, 3), max));
        }
    }
}

using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core
{
    internal class BeachCheckOperation
    {
        private int v;
        private NativeList<int2> positions;

        public BeachCheckOperation(int v, NativeList<int2> positions)
        {
            this.v = v;
            this.positions = positions;
        }
    }
}
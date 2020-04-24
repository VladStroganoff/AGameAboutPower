using Unity.Collections;
using Unity.Mathematics;

namespace TycoonTerrain.Core
{
    internal class BeachPaintOperation
    {
        private NativeList<int2> result;
        private int v;

        public BeachPaintOperation(NativeList<int2> result, int v)
        {
            this.result = result;
            this.v = v;
        }
    }
}
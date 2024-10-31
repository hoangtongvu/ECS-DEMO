using Core.Misc.FlowField;
using Unity.Burst;

namespace Utilities.Extensions
{
    [BurstCompile]
    public static class FlowFieldGridNodeExtension
    {
        [BurstCompile]
        public static bool IsPassable(in this FlowFieldGridNode node) => node.Cost != byte.MaxValue;

    }


}

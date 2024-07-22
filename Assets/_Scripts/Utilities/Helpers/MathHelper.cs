using Unity.Mathematics;

namespace Utilities.Helpers
{
    public static class MathHelper
    {

        public static float GetDistance2(in float3 pos1, in float3 pos2)
        {
            var deltaX = pos1.x - pos2.x;
            var deltaY = pos1.z - pos2.z;
            return math.sqrt(deltaX * deltaX + deltaY * deltaY);
        }

    }
}
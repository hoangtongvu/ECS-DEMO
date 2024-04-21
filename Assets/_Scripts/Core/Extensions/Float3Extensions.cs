using Unity.Mathematics;

namespace Core.Extensions
{
    public static class Float3Extensions
    {

        public static float3 Add(this float3 float3, float x = 0, float y = 0, float z = 0)
        {
            return new float3(float3.x + x, float3.y + y, float3.z + z);
        }

    }
}
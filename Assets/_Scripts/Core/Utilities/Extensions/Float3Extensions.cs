using Unity.Mathematics;

namespace Core.Utilities.Extensions
{
    public static class Float3Extensions
    {
        public static float3 Add(this float3 float3, float x = 0, float y = 0, float z = 0)
        {
            return new float3(float3.x + x, float3.y + y, float3.z + z);
        }

        public static float3 With(this float3 float3, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            return new float3(
                !float.IsNaN(x) ? x : float3.x,
                !float.IsNaN(y) ? y : float3.y,
                !float.IsNaN(z) ? z : float3.z
            );
        }

    }

}
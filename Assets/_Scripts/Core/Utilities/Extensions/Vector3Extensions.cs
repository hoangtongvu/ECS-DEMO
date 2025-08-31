using Unity.Mathematics;
using UnityEngine;

namespace Core.Utilities.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
        {
            return new Vector3(vector.x + x, vector.y + y, vector.z + z);
        }

        public static Vector3 With(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            return new float3(
                !float.IsNaN(x) ? x : vector.x,
                !float.IsNaN(y) ? y : vector.y,
                !float.IsNaN(z) ? z : vector.z
            );
        }

        public static void AssignAdd(ref this Vector3 vector, float x = 0, float y = 0, float z = 0)
        {
            vector = new Vector3(vector.x + x, vector.y + y, vector.z + z);
        }

        public static void AssignWith(ref this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            vector = new Vector3(
                !float.IsNaN(x) ? x : vector.x,
                !float.IsNaN(y) ? y : vector.y,
                !float.IsNaN(z) ? z : vector.z
            );
        }

        public static float3 ToFloat3(this Vector3 vector)
        {
            return new float3(vector.x, vector.y, vector.z);
        }

    }

}
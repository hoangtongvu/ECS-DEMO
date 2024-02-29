using Unity.Mathematics;
using UnityEngine;


public static class Vector3Extensions
{

    public static float3 ToFloat3(this Vector3 vector)
    {
        return new float3(vector.x, vector.y, vector.z);
    }

}

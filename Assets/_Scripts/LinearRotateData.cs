using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

public struct LinearRotateData : IComponentData
{
    public float3 direction;
    public float speed;
}

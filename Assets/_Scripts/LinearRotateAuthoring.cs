using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class LinearRotateAuthoring : MonoBehaviour
{
    public float3 direction = new float3(0, 1, 0);
    public float speed = 100;

    public class LinearRotateBaker : Baker<LinearRotateAuthoring>
    {
        public override void Bake(LinearRotateAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            LinearRotateData cubeData = new()
            {
                direction = authoring.direction,
                speed = authoring.speed,
            };
            AddComponent(entity, cubeData);
        }
    }
}

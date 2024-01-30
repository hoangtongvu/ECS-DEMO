using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class LinearRotateAuthoring : MonoBehaviour
{
    public float3 direction = new(0, 1, 0);
    public float speed = 100;

    public class LinearRotateBaker : Baker<LinearRotateAuthoring>
    {
        public override void Bake(LinearRotateAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Data data = new()
            {
                direction = authoring.direction,
                speed = authoring.speed,
            };
            AddComponent(entity, data);
        }
    }

    public struct Data : IComponentData
    {
        public float3 direction;
        public float speed;
    }

}

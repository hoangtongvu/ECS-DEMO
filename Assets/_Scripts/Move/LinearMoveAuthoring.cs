using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class LinearMoveAuthoring : MonoBehaviour
{
    public float3 direction;
    public float speed = 5;

    public class Baker : Baker<LinearMoveAuthoring>
    {
        public override void Bake(LinearMoveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Data data = new()
            {
                direction = new(
                    UnityEngine.Random.Range(-1f, 1f),
                    0,
                    UnityEngine.Random.Range(-1f, 1f)),
                speed = authoring.speed,
            };
            AddComponent(entity, data);
        }
    }

    public struct Data : IComponentData, IEnableableComponent
    {
        public float3 direction;
        public float speed;
    }

}

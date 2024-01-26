using System;
using Unity.Entities;
using UnityEngine;


public class CubeAuthoring : MonoBehaviour
{
    public float speed = 100;

    public class CubeBaker : Baker<CubeAuthoring>
    {
        public override void Bake(CubeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            CubeData cubeData = new()
            {
                speed = authoring.speed,
            };
            AddComponent(entity, cubeData);
        }
    }
}

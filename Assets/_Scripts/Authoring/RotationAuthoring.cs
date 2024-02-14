using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class RotationAuthoring : MonoBehaviour
    {
        public float3 direction = new(0f, 1f, 0f);
        public float speed = 100f;

        public class LinearRotateBaker : Baker<RotationAuthoring>
        {
            public override void Bake(RotationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RotateDirection
                {
                    value = math.normalizesafe(authoring.direction)
                });

                AddComponent(entity, new RotateSpeed
                {
                    value = authoring.speed
                });
            }
        }
    }
}

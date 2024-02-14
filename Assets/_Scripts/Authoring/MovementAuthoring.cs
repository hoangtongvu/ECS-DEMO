using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class MovementAuthoring : MonoBehaviour
    {
        public float3 direction;
        public float speed = 5f;

        private class Baker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<MoveDirection>(entity);

                AddComponent(entity, new MoveSpeed
                {
                    value = authoring.speed
                });
            }
        }
    }
}

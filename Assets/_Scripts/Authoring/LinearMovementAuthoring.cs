using Components.GameEntity.Movement;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class LinearMovementAuthoring : MonoBehaviour
    {
        public Vector3 Direction; // this is not necessary.
        public float Speed = 5f;
        public bool CanMoveEntity;

        private class Baker : Baker<LinearMovementAuthoring>
        {
            public override void Bake(LinearMovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<MoveDirectionFloat2>(entity);

                AddComponent(entity, new MoveSpeedLinear
                {
                    Value = authoring.Speed,
                });

                AddComponent<MoveableEntityTag>(entity);
                AddComponent<CanMoveEntityTag>(entity);
                SetComponentEnabled<CanMoveEntityTag>(entity, authoring.CanMoveEntity);

            }
        }
    }
}

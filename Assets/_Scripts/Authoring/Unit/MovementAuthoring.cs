using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class MovementAuthoring : MonoBehaviour
    {
        public Vector3 Direction; // this is not necessary.
        public Vector3 TargetPos;
        public float Speed = 5f;
        public float MinDistance = 1f;

        private class Baker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<MoveDirection>(entity);
                AddComponent(entity, new TargetPosition
                {
                    Value = authoring.TargetPos,
                });

                AddComponent(entity, new MoveSpeed
                {
                    Value = authoring.Speed,
                });

                AddComponent(entity, new MoveableState
                {
                    Entity = entity,
                });
                SetComponentEnabled<MoveableState>(entity, false);


                AddComponent(entity, new DistanceToTarget
                {
                    MinDistance = authoring.MinDistance,
                });


            }
        }
    }
}

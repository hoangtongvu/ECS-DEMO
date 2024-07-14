using Components;
using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class MoveTowardTargetAuthoring : MonoBehaviour
    {
        public Vector3 TargetPos;
        public float MinDistance = 1f;

        private class Baker : Baker<MoveTowardTargetAuthoring>
        {
            public override void Bake(MoveTowardTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new TargetPosition
                {
                    Value = authoring.TargetPos,
                });


                AddComponent(entity, new DistanceToTarget
                {
                    MinDistance = authoring.MinDistance,
                });

                AddComponent(entity, new MoveAffecterICD
                {
                    Value = Core.Unit.MoveAffecter.None,
                });


            }
        }
    }
}

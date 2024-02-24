using Components.Player;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring.Player
{
    public class MovementAuthoring : MonoBehaviour
    {
        public float moveSpeed = 5f;

        private class Baker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<MoveDirection>(entity);

                AddComponent(entity, new MoveSpeed
                {
                    Value = authoring.moveSpeed
                });
            }
        }
    }
}

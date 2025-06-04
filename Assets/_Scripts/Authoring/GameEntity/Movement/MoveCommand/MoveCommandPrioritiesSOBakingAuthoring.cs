using Components.GameEntity.Movement.MoveCommand;
using Core.GameEntity.Movement.MoveCommand;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.Movement.MoveCommand
{
    public class MoveCommandPrioritiesSOBakingAuthoring : MonoBehaviour
    {
        public MoveCommandPrioritiesSO MoveCommandPrioritiesSO;

        private class Baker : Baker<MoveCommandPrioritiesSOBakingAuthoring>
        {
            public override void Bake(MoveCommandPrioritiesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new MoveCommandPrioritiesSOHolder
                {
                    Value = authoring.MoveCommandPrioritiesSO,
                });

            }

        }

    }

}

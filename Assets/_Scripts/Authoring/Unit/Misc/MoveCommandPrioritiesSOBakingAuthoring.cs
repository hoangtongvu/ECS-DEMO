using Components.Unit.Misc;
using Core.Unit.Misc;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit.Misc
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

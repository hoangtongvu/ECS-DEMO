using Components.Unit.DarkUnit;
using Core.Unit.DarkUnit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit.DarkUnit
{
    public class DarkUnitConfigsSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private DarkUnitConfigsSO profilesSO;

        private class Baker : Baker<DarkUnitConfigsSOBakingAuthoring>
        {
            public override void Bake(DarkUnitConfigsSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new DarkUnitConfigsSOHolder
                {
                    Value = authoring.profilesSO,
                });

            }

        }

    }

}

using Authoring.Utilities.Helpers.GameEntity;
using Components.Unit;
using Core.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class UnitProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitProfilesSO profilesSO;

        private class Baker : Baker<UnitProfilesSOBakingAuthoring>
        {
            public override void Bake(UnitProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new UnitProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}

using Authoring.Utilities.Helpers.GameEntity;
using Components.Harvest;
using Core.Harvest;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Harvest
{
    public class HarvesteeProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private HarvesteeProfilesSO profilesSO;

        private class Baker : Baker<HarvesteeProfilesSOBakingAuthoring>
        {
            public override void Bake(HarvesteeProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new HarvesteeProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}

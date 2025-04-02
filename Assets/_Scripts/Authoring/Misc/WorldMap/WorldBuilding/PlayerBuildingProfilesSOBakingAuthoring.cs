using Authoring.Utilities.Helpers.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
using Core.Misc.WorldMap.WorldBuilding;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class PlayerBuildingProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private PlayerBuildingProfilesSO profilesSO;

        private class Baker : Baker<PlayerBuildingProfilesSOBakingAuthoring>
        {
            public override void Bake(PlayerBuildingProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new PlayerBuildingProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}

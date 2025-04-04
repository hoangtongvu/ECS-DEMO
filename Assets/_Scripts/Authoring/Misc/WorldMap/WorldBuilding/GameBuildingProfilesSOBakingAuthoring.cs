using Authoring.Utilities.Helpers.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
using Core.Misc.WorldMap.WorldBuilding;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class GameBuildingProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private GameBuildingProfilesSO profilesSO;

        private class Baker : Baker<GameBuildingProfilesSOBakingAuthoring>
        {
            public override void Bake(GameBuildingProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new GameBuildingProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}

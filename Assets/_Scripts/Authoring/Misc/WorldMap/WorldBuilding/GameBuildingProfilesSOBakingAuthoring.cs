using Authoring.Utilities.Helpers.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.Misc.WorldMap.WorldBuilding;
using Core.GameResource;
using Core.Misc.WorldMap.WorldBuilding;
using System;
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

                this.BakeSpawningProfiles(in entity, authoring.profilesSO);

            }

            private void BakeSpawningProfiles(in Entity bakerEntity, GameBuildingProfilesSO profilesSO)
            {
                var spawningProfilePictureBuffer = AddBuffer<LocalSpawningProfilePictureElement>(bakerEntity);
                var spawningDurationSecondsBuffer = AddBuffer<LocalSpawningDurationSecondsElement>(bakerEntity);
                var spawningCostBuffer = AddBuffer<LocalSpawningCostElement>(bakerEntity);
                int resourceCount = Enum.GetNames(typeof(ResourceType)).Length;

                foreach (var profile in profilesSO.Profiles)
                {
                    spawningProfilePictureBuffer.Add(new()
                    {
                        Value = profile.Value.ProfilePicture,
                    });

                    spawningDurationSecondsBuffer.Add(new()
                    {
                        Value = 0f,
                    });

                    for (int i = 0; i < resourceCount; i++)
                    {
                        profile.Value.BaseSpawningCosts.TryGetValue((ResourceType)i, out uint tempCost);
                        spawningCostBuffer.Add(new()
                        {
                            Value = tempCost,
                        });

                    }

                }

            }

        }

    }

}

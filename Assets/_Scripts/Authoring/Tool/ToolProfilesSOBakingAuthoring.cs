using Authoring.Utilities.Helpers.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.Tool;
using Core.GameResource;
using Core.Tool;
using System;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private ToolProfilesSO profilesSO;

        private class Baker : Baker<ToolProfilesSOBakingAuthoring>
        {
            public override void Bake(ToolProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ToolProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

                this.BakeSpawningProfiles(in entity, authoring.profilesSO);

            }

            private void BakeSpawningProfiles(in Entity bakerEntity, ToolProfilesSO profilesSO)
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
                        Value = profile.Value.SpawnDurationSeconds,
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

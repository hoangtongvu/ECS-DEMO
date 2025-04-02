using Authoring.Utilities.Helpers.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.Unit;
using Core.GameResource;
using Core.Unit;
using System;
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

                this.BakeSpawningProfiles(in entity, authoring.profilesSO);

            }

            private void BakeSpawningProfiles(in Entity bakerEntity, UnitProfilesSO profilesSO)
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

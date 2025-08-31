using Authoring.Utilities.Extensions;
using Authoring.Utilities.Helpers.GameBuilding;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProcess;
using Core.GameEntity;
using System;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.EntitySpawning
{
    public class EntitySpawningAuthoring : MonoBehaviour
    {
        public EntitySpawningProfilesSO SpawningProfiles;

        private class Baker : Baker<EntitySpawningAuthoring>
        {
            public override void Bake(EntitySpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                GameBuildingBakingHelper.AddComponents(this, entity);

                AddComponent<SpawnedEntityCounter>(entity);
                AddComponent(entity, new SpawnedEntityCountLimit
                {
                    Value = 3
                });

                this.AddAndDisableComponent<IsInSpawningProcessTag>(entity);
                this.AddAndDisableComponent<JustBeginSpawningProcessTag>(entity);
                this.AddAndDisableComponent<JustEndSpawningProcessTag>(entity);

                var buffer = AddBuffer<Components.GameEntity.EntitySpawning.EntitySpawningProfileElement>(entity);

                if (authoring.SpawningProfiles == null)
                    throw new NullReferenceException($"{nameof(authoring.SpawningProfiles)} is null");

                if (authoring.SpawningProfiles.UseAutoSpawnChances)
                    this.CheckAutoSpawnChances(authoring.SpawningProfiles);

                foreach (var profile in authoring.SpawningProfiles.Profiles)
                {
                    buffer.Add(new()
                    {
                        PrefabToSpawn = GetEntity(profile.PrefabToSpawn, TransformUsageFlags.Dynamic),
                        AutoSpawnChancePerTenThousand = profile.AutoSpawnChancePerTenThousand,
                        CanSpawnState = false,
                        SpawnCount = new()
                        {
                            Value = 0,
                            ValueChanged = false,
                        },
                        DurationCounterSeconds = 0f,
                    });

                }

            }

            private void CheckAutoSpawnChances(EntitySpawningProfilesSO SpawningProfiles)
            {
                const ushort targetChance = 10000;
                ushort totalChance = 0;

                foreach (var profile in SpawningProfiles.Profiles)
                {
                    totalChance += profile.AutoSpawnChancePerTenThousand;
                }

                if (totalChance == targetChance) return;

                Debug.LogError($"The totalChance: {totalChance} in {nameof(EntitySpawningProfilesSO)} is not 100%", SpawningProfiles);
            }

        }

    }

}

using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Unity.Collections;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using System.Collections.Generic;
using Components.GameEntity.EntitySpawning.SpawningProcess;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct DurationCountSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , IsInSpawningProcessTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<EntityToContainerIndexMap>();
            state.RequireForUpdate<EntitySpawningDurationsContainer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();

            var job = new CountUpJob()
            {
                EntityToContainerIndexMap = entityToContainerIndexMap,
                DurationsContainer = durationsContainer,
                DeltaTime = SystemAPI.Time.DeltaTime,
            };

            job.ScheduleParallel();
        }

        [WithAll(typeof(IsInSpawningProcessTag))]
        [BurstCompile]
        private partial struct CountUpJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public EntityToContainerIndexMap EntityToContainerIndexMap;
            [ReadOnly] public EntitySpawningDurationsContainer DurationsContainer;

            [BurstCompile]
            void Execute(
                ref DynamicBuffer<EntitySpawningProfileElement> profileElements)
            {
                for (int i = 0; i < profileElements.Length; i++)
                {
                    ref var profile = ref profileElements.ElementAt(i);

                    if (profile.SpawnCount.Value <= 0) continue;

                    float spawnDurationSeconds = this.GetDurationSeconds(in profile.PrefabToSpawn);

                    profile.DurationCounterSeconds += this.DeltaTime;

                    if (profile.DurationCounterSeconds >= spawnDurationSeconds)
                    {
                        profile.DurationCounterSeconds = 0;
                        profile.CanSpawnState = true;
                    }
                }

            }

            [BurstCompile]
            private float GetDurationSeconds(in Entity prefabToSpawnEntity)
            {
                if (!this.EntityToContainerIndexMap.Value.TryGetValue(prefabToSpawnEntity, out int containerIndex))
                    throw new KeyNotFoundException($"{nameof(EntityToContainerIndexMap)} does not contain key: {prefabToSpawnEntity}");

                return this.DurationsContainer.Value[containerIndex];
            }

        }

    }

}
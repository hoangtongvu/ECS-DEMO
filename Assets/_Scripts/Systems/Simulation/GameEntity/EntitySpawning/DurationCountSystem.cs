using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Unity.Collections;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using System.Collections.Generic;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct DurationCountSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitySpawningProfileElement>();
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

        [BurstCompile]
        private partial struct CountUpJob : IJobEntity
        {
            public float DeltaTime;
            [ReadOnly] public EntityToContainerIndexMap EntityToContainerIndexMap;
            [ReadOnly] public EntitySpawningDurationsContainer DurationsContainer;

            [BurstCompile]
            void Execute(
                ref DynamicBuffer<EntitySpawningProfileElement> profileElements
                , in Entity entity)
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
                {
                    UnityEngine.Debug.Log(this.EntityToContainerIndexMap.Value.Count);
                    throw new KeyNotFoundException($"{nameof(EntityToContainerIndexMap)} does not contain key: {prefabToSpawnEntity}");
                }

                return this.DurationsContainer.Value[containerIndex];
            }

        }

    }

}
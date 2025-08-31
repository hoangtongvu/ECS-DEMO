using Components.GameEntity.EntitySpawning;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Utilities.Extensions;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct AutoIncSpawnCountSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new(47);

            var entityQuery0 = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , SpawnedEntityCounter
                    , SpawnedEntityCountLimit
                    , SpawnerAutoSpawnTag>()
                    .Build();

            state.RequireForUpdate(entityQuery0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (spawningProfiles, spawnedEntityCounterRef, spawnedEntityCountLimitRef) in SystemAPI
                .Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<SpawnedEntityCounter>
                    , RefRO<SpawnedEntityCountLimit>>()
                .WithAll<SpawnerAutoSpawnTag>())
            {
                int spawnCountLeft = spawnedEntityCountLimitRef.ValueRO.Value - spawnedEntityCounterRef.ValueRO.Value;

                while (spawnCountLeft > 0)
                {
                    int randomIndex = this.DrawRandomIndexFromProfiles(in spawningProfiles);
                    ref var profile = ref spawningProfiles.ElementAt(randomIndex);

                    profile.SpawnCount.ChangeValue(profile.SpawnCount.Value + 1);
                    spawnedEntityCounterRef.ValueRW.Value++;

                    spawnCountLeft--;
                }

            }

        }

        [BurstCompile]
        private int DrawRandomIndexFromProfiles(in DynamicBuffer<EntitySpawningProfileElement> profiles)
        {
            const ushort maxChance = 10000;
            uint drawnValue = this.rand.NextUInt(0, maxChance);

            ushort cumulative = 0;
            int profileCount = profiles.Length;

            for (int i = 0; i < profileCount; i++)
            {
                cumulative += profiles[i].AutoSpawnChancePerTenThousand;

                if (drawnValue >= cumulative) continue;
                return i;
            }

            return -1;
        }

    }

}

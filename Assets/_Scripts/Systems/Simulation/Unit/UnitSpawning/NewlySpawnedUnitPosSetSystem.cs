using Unity.Entities;
using Unity.Burst;
using Components.Unit.UnitSpawning;
using Components;
using Unity.Transforms;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem_MultiType))]
    [BurstCompile]
    public partial struct NewlySpawnedUnitPosSetSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<NewlySpawned>();
            // state.RequireForUpdate<SelfEntityRef>();
            // state.RequireForUpdate<LocalTransform>();

            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , SelfEntityRef
                    , LocalTransform
                    , SpawnPos>()
                .Build();
            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Can't put NewlySpawned component lookup into a IJobEntity that Query NewlySpawned.
            foreach (var (spawnPos, selfEntityRef, transformRef) in
                SystemAPI.Query<
                    RefRO<SpawnPos>
                    , RefRO<SelfEntityRef>
                    , RefRW<LocalTransform>>()
                    .WithAll<NewlySpawnedTag>())
            {
                transformRef.ValueRW.Position = spawnPos.ValueRO.Value;
                SystemAPI.SetComponentEnabled<NewlySpawnedTag>(selfEntityRef.ValueRO.Value, false);
            }

        }

    }
}
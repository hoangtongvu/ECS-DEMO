using Unity.Entities;
using Unity.Burst;
using Components.MyEntity.EntitySpawning;
using Unity.Transforms;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem))]
    [BurstCompile]
    public partial struct NewlySpawnedUnitPosSetSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , LocalTransform
                    , SpawnPos>()
                .Build();
            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (spawnPos, transformRef) in
                SystemAPI.Query<
                    RefRO<SpawnPos>
                    , RefRW<LocalTransform>>()
                    .WithAll<NewlySpawnedTag>())
            {
                transformRef.ValueRW.Position = spawnPos.ValueRO.Value;
            }

        }

    }
}
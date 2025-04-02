using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Core.Utilities.Extensions;
using Components.GameEntity.EntitySpawning;

namespace Systems.Simulation.GameEntity.EntitySpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem))]
    [BurstCompile]
    public partial struct NewlySpawnedUnitPosSetSystem : ISystem
    {
        private Random rand;
        private float2 tempVector;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new Random(1);
            this.tempVector = new(1, 1);


            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , LocalTransform
                    , SpawnerPos>()
                .Build();
            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (spawnerPos, transformRef) in
                SystemAPI.Query<
                    RefRO<SpawnerPos>
                    , RefRW<LocalTransform>>()
                    .WithAll<NewlySpawnedTag>())
            {
                transformRef.ValueRW.Position = this.GetRandomPositionInRadius(3, spawnerPos.ValueRO.Value);
            }

        }

        private float3 GetRandomPositionInRadius(in float radius, in float3 centerPos)
        {
            // This is heavy work?? try to use job.
            float2 distanceVector = math.normalize(this.rand.NextFloat2(-this.tempVector, this.tempVector)) * radius;
            return centerPos.Add(x: distanceVector.x, z: distanceVector.y);
        }

    }
}
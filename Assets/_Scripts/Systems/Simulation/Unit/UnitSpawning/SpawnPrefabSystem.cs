using Unity.Entities;
using Unity.Burst;
using Components.Unit.UnitSpawning;
using Unity.Transforms;
using Unity.Mathematics;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SpawnPrefabSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new Random(1);
            state.RequireForUpdate<PrefabToSpawn>();
            state.RequireForUpdate<CanSpawnState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (prefabRef, spawnStateRef, spawnRadiusRef, transformRef) in
                SystemAPI.Query<
                    RefRO<PrefabToSpawn>
                    , RefRW<CanSpawnState>
                    , RefRO<SpawnRadius>
                    , RefRO<LocalTransform>>())
            {
                if (!spawnStateRef.ValueRO.Value) continue;
                spawnStateRef.ValueRW.Value = false;

                Entity entity = state.EntityManager.Instantiate(prefabRef.ValueRO.Value);
                RefRW<SpawnPos> spawnPosRef = SystemAPI.GetComponentRW<SpawnPos>(entity);
                spawnPosRef.ValueRW.Value = this.GetRandomPositionInRadius(spawnRadiusRef.ValueRO.Value, transformRef.ValueRO.Position);
            }
        }

        private float3 GetRandomPositionInRadius(in float radius, in float3 centerPos)
        {
            // This is heavy work?? try to use job.
            float2 distanceVector = math.normalize(this.rand.NextFloat2()) * radius;
            return centerPos.Add(x: distanceVector.x, z: distanceVector.y);
        }

    }
}
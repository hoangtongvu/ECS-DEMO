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
        private float2 tempVector;
        private EntityQuery spawnerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new Random(1);
            this.tempVector = new(1, 1);

            this.spawnerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSpawningProfileElement
                    , SpawnRadius
                    , LocalTransform>()
                .Build();

            state.RequireForUpdate(this.spawnerQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (profiles, spawnRadiusRef, transformRef) in
                SystemAPI.Query<
                    DynamicBuffer<UnitSpawningProfileElement>
                    , RefRO<SpawnRadius>
                    , RefRO<LocalTransform>>())
            {
                
                for (int i = 0; i < profiles.Length; i++)
                {
                    ref var profile = ref profiles.ElementAt(i);

                    if (!profile.CanSpawnState) continue;
                    profile.CanSpawnState = false;

                    Entity entity = state.EntityManager.Instantiate(profile.PrefabToSpawn);
                    RefRW<SpawnPos> spawnPosRef = SystemAPI.GetComponentRW<SpawnPos>(entity);
                    spawnPosRef.ValueRW.Value = this.GetRandomPositionInRadius(spawnRadiusRef.ValueRO.Value, transformRef.ValueRO.Position);
                }

            }
        }

        private float3 GetRandomPositionInRadius(in float radius, in float3 centerPos)
        {
            // This is heavy work?? try to use job.
            float2 distanceVector = math.normalize(this.rand.NextFloat2(- this.tempVector, this.tempVector)) * radius;
            return centerPos.Add(x: distanceVector.x, z: distanceVector.y);
        }

    }
}
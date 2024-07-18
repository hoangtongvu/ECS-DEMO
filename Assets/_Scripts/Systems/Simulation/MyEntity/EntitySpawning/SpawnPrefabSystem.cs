using Unity.Entities;
using Unity.Burst;
using Components.MyEntity.EntitySpawning;
using Unity.Transforms;
using Components;

namespace Systems.Simulation.MyEntity.EntitySpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SpawnPrefabSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var spawnerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , LocalTransform>()
                .Build();

            state.RequireForUpdate(spawnerQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (profiles, transformRef, selfEntity) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<LocalTransform>>()
                    .WithEntityAccess())
            {
                
                for (int i = 0; i < profiles.Length; i++)
                {
                    ref var profile = ref profiles.ElementAt(i);

                    if (!profile.CanSpawnState) continue;
                    profile.CanSpawnState = false;

                    profile.SpawnCount.ChangeValue(profile.SpawnCount.Value - 1);

                    Entity entity = state.EntityManager.Instantiate(profile.PrefabToSpawn);
                    
                    if (SystemAPI.HasComponent<SpawnerPos>(entity))
                    {
                        var spawnerPosRef = SystemAPI.GetComponentRW<SpawnerPos>(entity);
                        spawnerPosRef.ValueRW.Value = transformRef.ValueRO.Position;
                    }

                    
                    if (SystemAPI.HasComponent<SpawnerEntityRef>(entity))
                    {
                        var spawnerEntityRef = SystemAPI.GetComponentRW<SpawnerEntityRef>(entity);
                        spawnerEntityRef.ValueRW.Value = selfEntity;
                    }

                    
                }

            }
        }


    }
}
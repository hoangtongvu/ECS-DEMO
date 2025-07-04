using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(NewlySpawnedTagClearSystem))]
    [BurstCompile]
    public partial struct NewlySpawnedUnitPosSetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , NeedInitPosAroundSpawnerTag
                    , LocalTransform
                    , SpawnerEntityHolder>()
                .Build();

            state.RequireForUpdate(entityQuery);
            state.RequireForUpdate<SetPosWithinRadiusCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandList = SystemAPI.GetSingleton<SetPosWithinRadiusCommandList>().Value;
            var entities = new NativeList<Entity>(10, Allocator.Temp);

            foreach (var (spawnerEntityHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<SpawnerEntityHolder>>()
                .WithAll<
                    NewlySpawnedTag
                    , NeedInitPosAroundSpawnerTag>()
                .WithEntityAccess())
            {
                entities.Add(entity);
                var spawnerEntity = spawnerEntityHolderRef.ValueRO.Value;
                if (spawnerEntity == Entity.Null) continue;

                commandList.Add(new()
                {
                    BaseEntity = entity,
                    CenterPos = SystemAPI.GetComponent<LocalTransform>(spawnerEntity).Position,
                    Radius = 3f,
                });
            }

            state.EntityManager.RemoveComponent<NeedInitPosAroundSpawnerTag>(entities.AsArray());

        }

    }

}
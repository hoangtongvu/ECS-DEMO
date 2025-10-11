using Components.GameEntity;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
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
                    , SpawnerEntityHolder
                    , PrimaryPrefabEntityHolder>()
                .Build();

            state.RequireForUpdate(entityQuery);
            state.RequireForUpdate<GameEntitySizeMap>();
            state.RequireForUpdate<SetPosWithinRadiusCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>().Value;
            var commandList = SystemAPI.GetSingleton<SetPosWithinRadiusCommandList>().Value;
            var entities = new NativeList<Entity>(10, Allocator.Temp);

            foreach (var (spawnerEntityHolderRef, primaryEntityHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<SpawnerEntityHolder>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                .WithAll<
                    NewlySpawnedTag
                    , NeedInitPosAroundSpawnerTag>()
                .WithEntityAccess())
            {
                entities.Add(entity);
                var spawnerEntity = spawnerEntityHolderRef.ValueRO.Value;
                if (spawnerEntity == Entity.Null) continue;

                var gameEntitySize = gameEntitySizeMap[primaryEntityHolderRef.ValueRO];

                commandList.Add(new()
                {
                    BaseEntity = entity,
                    OffsetYFromGround = gameEntitySize.ObjectHeight,
                    CenterPos = SystemAPI.GetComponent<LocalTransform>(spawnerEntity).Position,
                    Radius = 3f,
                });
            }

            state.EntityManager.RemoveComponent<NeedInitPosAroundSpawnerTag>(entities.AsArray());
        }

    }

}
using Components.GameEntity;
using Components.GameEntity.Misc;
using Components.GameState;
using Components.Player;
using Components.Tool;
using Components.Tool.Misc;
using Components.Unit.DarkUnit;
using Components.Unit.Misc;
using Core.GameEntity;
using Core.Tool;
using Core.Unit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities;

namespace Systems.Initialization.Unit.DarkUnit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct DarkUnitWaveSpawnSystem : ISystem
    {
        private EntityQuery playerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);
            su.AddComponent<DarkUnitSpawnCycleCounter>();
            su.AddComponent<LatestDarkUnitSpawnTimestamp>();

            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , LocalTransform>()
                .Build();

            state.RequireForUpdate(this.playerQuery);
            state.RequireForUpdate<DarkUnitProfileMap>();
            state.RequireForUpdate<DarkUnitSpawnRadius>();
            state.RequireForUpdate<DarkUnitSpawnDurationMinutes>();
            state.RequireForUpdate<UnitProfileId2PrimaryPrefabEntityMap>();
            state.RequireForUpdate<ToolProfileId2PrimaryEntityMap>();
            state.RequireForUpdate<SetPosWithinRadiusCommandList>();
            state.RequireForUpdate<GameEntitySizeMap>();
            state.RequireForUpdate<IsGameStarted>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var latestDarkUnitSpawnTimestampRef = SystemAPI.GetSingletonRW<LatestDarkUnitSpawnTimestamp>();
            half spawnDurationMinutes = SystemAPI.GetSingleton<DarkUnitSpawnDurationMinutes>().Value;

            var elapsedTimeSeconds = SystemAPI.Time.ElapsedTime - latestDarkUnitSpawnTimestampRef.ValueRO.Value;
            if (elapsedTimeSeconds / 60 < spawnDurationMinutes) return;

            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>().Value;
            var spawnProfileMap = SystemAPI.GetSingleton<DarkUnitProfileMap>().Value;
            half spawnRadius = SystemAPI.GetSingleton<DarkUnitSpawnRadius>().Value;

            var spawnCycleCounterRef = SystemAPI.GetSingletonRW<DarkUnitSpawnCycleCounter>();
            var unitProfileId2PrimaryPrefabEntityMap = SystemAPI.GetSingleton<UnitProfileId2PrimaryPrefabEntityMap>().Value;
            var toolProfileId2PrimaryEntityMap = SystemAPI.GetSingleton<ToolProfileId2PrimaryEntityMap>().Value;
            var setPosWithinRadiusCommands = SystemAPI.GetSingleton<SetPosWithinRadiusCommandList>().Value;
            var em = state.EntityManager;

            latestDarkUnitSpawnTimestampRef.ValueRW.Value = SystemAPI.Time.ElapsedTime;
            spawnCycleCounterRef.ValueRW.Value++;

            foreach (var kVPair in spawnProfileMap)
            {
                var spawnProfile = kVPair.Value;
                int spawnCount = (int)math.floor(spawnCycleCounterRef.ValueRO.Value * spawnProfile.SpawnRate);

                this.SpawnUnitsOfTheSameType(
                    ref state
                    , in em
                    , in unitProfileId2PrimaryPrefabEntityMap
                    , in gameEntitySizeMap
                    , in setPosWithinRadiusCommands
                    , kVPair.Key
                    , in spawnRadius
                    , in spawnCount
                    , out var newUnitEntities);

                this.SpawnToolOfTheSameType(
                    ref state
                    , in em
                    , in toolProfileId2PrimaryEntityMap
                    , in spawnProfile.ToolProfileId
                    , in spawnCount
                    , out var newToolEntities);

                for (int i = 0; i < spawnCount; i++)
                {
                    this.MarkToolCanBePicked(ref state, newToolEntities[i], newUnitEntities[i]);
                }

            }

        }

        [BurstCompile]
        private void SpawnUnitsOfTheSameType(
            ref SystemState state
            , in EntityManager em
            , in NativeHashMap<UnitProfileId, Entity> unitProfileId2PrimaryPrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , in NativeList<SetPosWithinRadiusCommand> setPosWithinRadiusCommands
            , in UnitProfileId prefabId
            , in half spawnRadius
            , in int spawnCount
            , out NativeArray<Entity> newUnitEntities)
        {
            em.CompleteDependencyBeforeRO<LocalTransform>();

            float3 playerPos = this.playerQuery.GetSingleton<LocalTransform>().Position;
            Entity prefabToSpawn = unitProfileId2PrimaryPrefabEntityMap[prefabId];
            var gameEntitySize = gameEntitySizeMap[prefabToSpawn];

            newUnitEntities = em.Instantiate(prefabToSpawn, spawnCount, Allocator.Temp);

            for (int i = 0; i < spawnCount; i++)
            {
                setPosWithinRadiusCommands.Add(new()
                {
                    BaseEntity = newUnitEntities[i],
                    OffsetYFromGround = gameEntitySize.ObjectHeight,
                    CenterPos = playerPos,
                    Radius = spawnRadius,
                });
            }
        }

        [BurstCompile]
        private void SpawnToolOfTheSameType(
            ref SystemState state
            , in EntityManager em
            , in NativeHashMap<ToolProfileId, Entity> toolProfileId2PrimaryEntityMap
            , in ToolProfileId prefabId
            , in int spawnCount
            , out NativeArray<Entity> newToolEntities)
        {
            Entity prefabToSpawn = toolProfileId2PrimaryEntityMap[prefabId];
            newToolEntities = em.Instantiate(prefabToSpawn, spawnCount, Allocator.Temp);
        }

        [BurstCompile]
        private void MarkToolCanBePicked(ref SystemState state, in Entity toolEntity, in Entity unitEntity)
        {
            SystemAPI.SetComponentEnabled<CanBePickedTag>(toolEntity, true);

            var toolPickerEntityRef = SystemAPI.GetComponentRW<ToolPickerEntity>(toolEntity);
            toolPickerEntityRef.ValueRW.Value = unitEntity;
        }

    }

}
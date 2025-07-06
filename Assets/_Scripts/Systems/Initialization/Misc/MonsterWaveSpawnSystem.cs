using Components.GameEntity.Misc;
using Components.Player;
using Components.Tool;
using Components.Tool.Misc;
using Components.Unit.Misc;
using Core.Tool;
using Core.Unit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Initialization.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MonsterWaveSpawnSystemGroup : ComponentSystemGroup
    {
        public MonsterWaveSpawnSystemGroup()
        {
            this.RateManager = new RateUtils.VariableRateManager(5000);
        }
    }

    [UpdateInGroup(typeof(MonsterWaveSpawnSystemGroup))]
    [BurstCompile]
    public partial struct MonsterWaveSpawnSystem : ISystem
    {
        private EntityQuery playerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , LocalTransform>()
                .Build();

            state.RequireForUpdate(this.playerQuery);
            state.RequireForUpdate<UnitProfileId2PrimaryPrefabEntityMap>();
            state.RequireForUpdate<ToolProfileId2PrimaryEntityMap>();
            state.RequireForUpdate<SetPosWithinRadiusCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitProfileId2PrimaryPrefabEntityMap = SystemAPI.GetSingleton<UnitProfileId2PrimaryPrefabEntityMap>().Value;
            var toolProfileId2PrimaryEntityMap = SystemAPI.GetSingleton<ToolProfileId2PrimaryEntityMap>().Value;
            var setPosWithinRadiusCommands = SystemAPI.GetSingleton<SetPosWithinRadiusCommandList>().Value;
            var em = state.EntityManager;

            this.SpawnUnit(ref state, in em, in unitProfileId2PrimaryPrefabEntityMap, in setPosWithinRadiusCommands, out var newUnitEntity);
            this.SpawnTool(ref state, in em, in toolProfileId2PrimaryEntityMap, out var newToolEntity);

            this.MarkToolCanBePicked(ref state, in newToolEntity, in newUnitEntity);

        }

        [BurstCompile]
        private void SpawnUnit(
            ref SystemState state
            , in EntityManager em
            , in NativeHashMap<UnitProfileId, Entity> unitProfileId2PrimaryPrefabEntityMap
            , in NativeList<SetPosWithinRadiusCommand> setPosWithinRadiusCommands
            , out Entity newUnitEntity)
        {
            var prefabId = new UnitProfileId
            {
                UnitType = UnitType.Knight,
                VariantIndex = 10,
            };

            em.CompleteDependencyBeforeRO<LocalTransform>();

            float3 playerPos = this.playerQuery.GetSingleton<LocalTransform>().Position;
            Entity prefabToSpawn = unitProfileId2PrimaryPrefabEntityMap[prefabId];

            newUnitEntity = em.Instantiate(prefabToSpawn);

            setPosWithinRadiusCommands.Add(new()
            {
                BaseEntity = newUnitEntity,
                CenterPos = playerPos,
                Radius = 30f,
            });

        }

        [BurstCompile]
        private void SpawnTool(
            ref SystemState state
            , in EntityManager em
            , in NativeHashMap<ToolProfileId, Entity> toolProfileId2PrimaryEntityMap
            , out Entity newToolEntity)
        {
            var prefabId = new ToolProfileId
            {
                ToolType = ToolType.Sword,
                VariantIndex = 0,
            };

            Entity prefabToSpawn = toolProfileId2PrimaryEntityMap[prefabId];

            newToolEntity = em.Instantiate(prefabToSpawn);

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
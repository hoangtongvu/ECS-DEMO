using Components.GameEntity.EntitySpawning;
using Components.Misc;
using Components.Tool;
using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Systems.Simulation.Tool;
using Utilities.Extensions.GameEntity.EntitySpawning;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Simulation.UnitAndTool
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolAssignSystem))]
    [BurstCompile]
    public partial struct ToolPickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    SpawnerEntityHolder
                    , CanBePickedTag
                    , ToolPickerEntity
                    , DerelictToolTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<UnitToolHolder>();
            state.RequireForUpdate<SpawnedEntityCounter>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var toolStatsMap = SystemAPI.GetSingleton<ToolStatsMap>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transformRef, spawnerEntityHolderRef, toolProfileIdHolder, canBePickedTagRef, toolPickerEntityRef, toolEntity) in
                SystemAPI.Query<
                    RefRW<LocalTransform>
                    , RefRW<SpawnerEntityHolder>
                    , RefRO<ToolProfileIdHolder>
                    , EnabledRefRW<CanBePickedTag>
                    , RefRW<ToolPickerEntity>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {
                this.HandleUnit(
                    ref state
                    , in toolStatsMap
                    , ecb
                    , toolPickerEntityRef.ValueRO.Value
                    , toolEntity
                    , in toolProfileIdHolder.ValueRO);

                this.HandleToolSpawner(
                    ref state
                    , ref spawnerEntityHolderRef.ValueRW
                    , in toolEntity);

                this.HandleTool(
                    ref state
                    , transformRef
                    , ecb
                    , canBePickedTagRef
                    , toolPickerEntityRef
                    , toolEntity);

            }

            ecb.Playback(state.EntityManager);

        }

        [BurstCompile]
        private void HandleToolSpawner(
            ref SystemState state
            , ref SpawnerEntityHolder spawnerEntityHolder
            , in Entity toolEntity)
        {
            var spawnerEntity = spawnerEntityHolder.Value;

            if (SystemAPI.HasComponent<SpawnedEntityCounter>(spawnerEntity))
            {
                var spawnedEntityCounterRef = SystemAPI.GetComponentRW<SpawnedEntityCounter>(spawnerEntity);
                spawnedEntityCounterRef.ValueRW.Value--;
            }

            if (SystemAPI.HasComponent<SpawnedEntityArray>(spawnerEntity))
            {
                var spawnedEntities = SystemAPI.GetComponent<SpawnedEntityArray>(spawnerEntity);
                spawnedEntities.Remove(in toolEntity);
            }
            
            spawnerEntityHolder.Value = Entity.Null;
        }

        [BurstCompile]
        private void HandleTool(
            ref SystemState state
            , RefRW<LocalTransform> transformRef
            , EntityCommandBuffer ecb
            , EnabledRefRW<CanBePickedTag> canBePickedTagRef
            , RefRW<ToolPickerEntity> toolPickerEntityRef
            , Entity toolEntity)
        {
            // Remove Gravity of tool
            ecb.AddComponent(toolEntity, new PhysicsGravityFactor
            {
                Value = 0
            });

            // Hide tool out of view
            var unitTransform = SystemAPI.GetComponentRO<LocalTransform>(toolPickerEntityRef.ValueRO.Value);
            transformRef.ValueRW.Position = unitTransform.ValueRO.Position.Add(y: 100f);
            transformRef.ValueRW.Scale = 0;
            transformRef.ValueRW.Rotation = quaternion.identity;

            SystemAPI.SetComponentEnabled<DerelictToolTag>(toolEntity, false);
            canBePickedTagRef.ValueRW = false;
            toolPickerEntityRef.ValueRW.Value = Entity.Null;
        }

        [BurstCompile]
        private void HandleUnit(
            ref SystemState state
            , in ToolStatsMap toolStatsMap
            , EntityCommandBuffer ecb
            , in Entity unitEntity
            , in Entity toolEntity
            , in ToolProfileIdHolder toolProfileIdHolder)
        {
            var unitToolHolderRef = SystemAPI.GetComponentRW<UnitToolHolder>(unitEntity);
            var toolProfileIdHolderRef = SystemAPI.GetComponentRW<ToolProfileIdHolder>(unitEntity);

            SetToolHolder(
                ref unitToolHolderRef.ValueRW
                , new UnitToolHolder { Value = toolEntity }
                , ref toolProfileIdHolderRef.ValueRW
                , in toolProfileIdHolder);

            var toolStats = toolStatsMap.Value[toolProfileIdHolder.Value];

            SystemAPI.GetComponentRW<BaseDmg>(unitEntity).ValueRW.Value = toolStats.BaseDmg;
            SystemAPI.GetComponentRW<BaseWorkSpeed>(unitEntity).ValueRW.Value = toolStats.BaseWorkSpeed;

            RemoveJoblessUnitTag(in ecb, in unitEntity);

            ecb.AddComponent<NeedRoleUpdatedTag>(unitEntity);
            ecb.AddComponent<NeedInitArmedStateComponentsTag>(unitEntity);

        }

    }

}
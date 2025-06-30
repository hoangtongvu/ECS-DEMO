using Components.GameEntity;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Components.Tool;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Core.GameEntity.Movement.MoveCommand;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Utilities.Jobs;

namespace Systems.Simulation.UnitAndTool
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ToolCallUnitSystemGroup : ComponentSystemGroup
    {
        public ToolCallUnitSystemGroup()
        {
            this.RateManager = new RateUtils.VariableRateManager(1000);
        }
    }

    [UpdateInGroup(typeof(ToolCallUnitSystemGroup))]
    [BurstCompile]
    public partial struct ToolCallUnitSystem : ISystem
    {
        private EntityQuery toolQuery;
        private EntityQuery unitQuery;
        private EntityQuery setCanOverrideMoveCommandTagJobQuery;
        private EntityQuery setTargetJobQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.toolQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , ToolPickerEntity
                    , DerelictToolTag>()
                .WithPresent<CanBePickedTag>()
                .Build();

            this.unitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , JoblessUnitTag
                    , LocalTransform
                    , FactionIndex
                    , InteractingEntity
                    , MoveCommandElement>()
                .WithPresent<
                    CanSetTargetJobScheduleTag>()
                .Build();

            this.setCanOverrideMoveCommandTagJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , JoblessUnitTag>()
                .WithAll<
                    MoveCommandElement
                    , InteractingEntity
                    , InteractionTypeICD
                    , ArmedStateHolder>()
                .WithAll<
                    CanSetTargetJobScheduleTag>()
                .WithPresent<
                    CanOverrideMoveCommandTag>()
                .Build();

            this.setTargetJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , JoblessUnitTag>()
                .WithAll<
                    LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , MoveSpeedLinear
                    , TargetEntity
                    , TargetEntityWorldSquareRadius
                    , MoveCommandElement
                    , InteractableDistanceRange>()
                .WithAll<
                    CanSetTargetJobScheduleTag
                    , CanOverrideMoveCommandTag>()
                .WithPresent<
                    CanFindPathTag>()
                .Build();

            state.RequireForUpdate(this.setCanOverrideMoveCommandTagJobQuery);
            state.RequireForUpdate(this.setTargetJobQuery);
            state.RequireForUpdate<MoveCommandPrioritiesMap>();
            state.RequireForUpdate<GameEntitySizeMap>();
            state.RequireForUpdate<CellRadius>();
            state.RequireForUpdate<UnitReactionConfigsMap>();
            state.RequireForUpdate<DetectionRadiusMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>();
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;
            var detectionRadiusMap = SystemAPI.GetSingleton<DetectionRadiusMap>();

            var toolEntities = this.toolQuery.ToEntityArray(Allocator.TempJob);
            var toolTransforms = this.toolQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            int toolCount = toolEntities.Length;

            var unitEntities = this.unitQuery.ToEntityArray(Allocator.TempJob);
            int unitCount = unitEntities.Length;

            var targetEntities = new NativeArray<Entity>(unitCount, Allocator.TempJob);
            var targetPositions = new NativeArray<float3>(unitCount, Allocator.TempJob);
            var targetInfoMap = new NativeHashMap<Entity, TargetEntityInfo>(unitCount, Allocator.TempJob);

            state.Dependency = new GetTargetEntitiesAndPositionsJob
            {
                ToolCount = toolCount,
                ToolEntities = toolEntities,
                ToolTransforms = toolTransforms,
                DetectionRadiusMap = detectionRadiusMap,
                TargetEntities = targetEntities,
                TargetPositions = targetPositions,
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new InitTargetInfoMapJob
            {
                MainEntityArray = unitEntities,
                TargetEntities = targetEntities,
                TargetPositions = targetPositions,
                TargetInfoMap = targetInfoMap,
            }.Schedule(state.Dependency);

            state.Dependency = new SetCanOverrideMoveCommandTagJob
            {
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                NewMoveCommandSource = MoveCommandSource.ToolCall,
            }.ScheduleParallel(this.setCanOverrideMoveCommandTagJobQuery, state.Dependency);

            state.Dependency = new SetSpeedsAsRunSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new SetMultipleTargetsJob
            {
                MainEntityAndTargetInfoMap = targetInfoMap,
                PrimaryPrefabEntityHolderLookup = SystemAPI.GetComponentLookup<PrimaryPrefabEntityHolder>(),
                GameEntitySizeMap = gameEntitySizeMap,
                CellRadius = cellRadius,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new CleanTagsJob().ScheduleParallel(state.Dependency);

            state.Dependency = toolEntities.Dispose(state.Dependency);
            state.Dependency = toolTransforms.Dispose(state.Dependency);
            state.Dependency = unitEntities.Dispose(state.Dependency);
            state.Dependency = targetEntities.Dispose(state.Dependency);
            state.Dependency = targetPositions.Dispose(state.Dependency);
            state.Dependency = targetInfoMap.Dispose(state.Dependency);

        }

        //[WithAll(typeof(JoblessUnitTag))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct GetTargetEntitiesAndPositionsJob : IJobEntity
        {
            [ReadOnly] public int ToolCount;
            [ReadOnly] public NativeArray<Entity> ToolEntities;
            [ReadOnly] public NativeArray<LocalTransform> ToolTransforms;

            [ReadOnly] public DetectionRadiusMap DetectionRadiusMap;

            [WriteOnly] public NativeArray<Entity> TargetEntities;
            [WriteOnly] public NativeArray<float3> TargetPositions;

            void Execute(
                in UnitProfileIdHolder unitProfileIdHolder
                , in LocalTransform unitTransform
                , EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
                , EnabledRefRO<JoblessUnitTag> joblessUnitTag
                , in InteractingEntity interactingEntity
                , in MoveCommandElement moveCommandElement
                , [EntityIndexInQuery] int entityIndex)
            {
                if (!joblessUnitTag.ValueRO) return;
                if (interactingEntity.Value != Entity.Null) return;
                if (moveCommandElement.TargetEntity != Entity.Null) return;

                half detectionRadius = this.DetectionRadiusMap.Value[unitProfileIdHolder.Value];

                bool canGetToolIndex =
                    this.TryGetNearestValidToolIndex(in unitTransform.Position, in detectionRadius, out int toolIndex);

                if (!canGetToolIndex) return;

                this.TargetEntities[entityIndex] = this.ToolEntities[toolIndex];
                this.TargetPositions[entityIndex] = this.ToolTransforms[toolIndex].Position;
                canSetTargetJobScheduleTag.ValueRW = true;

            }

            [BurstCompile]
            private bool TryGetNearestValidToolIndex(
                in float3 unitPos
                , in half detectionRadius
                , out int index)
            {
                float radiusSq = math.square(detectionRadius);
                index = -1;
                float minDistanceSq = float.MaxValue;

                for (int i = 0; i < this.ToolCount; i++)
                {
                    float distanceSq = math.distancesq(this.ToolTransforms[i].Position, unitPos);
                    if (distanceSq > radiusSq) continue;
                    if (distanceSq > minDistanceSq) continue;

                    minDistanceSq = distanceSq;
                    index = i;
                }

                return index != -1;
            }

        }

        [BurstCompile]
        private partial struct InitTargetInfoMapJob : IJob
        {
            [ReadOnly] public NativeArray<Entity> MainEntityArray;
            [ReadOnly] public NativeArray<Entity> TargetEntities;
            [ReadOnly] public NativeArray<float3> TargetPositions;

            [WriteOnly] public NativeHashMap<Entity, TargetEntityInfo> TargetInfoMap;

            [BurstCompile]
            public void Execute()
            {
                int length = this.MainEntityArray.Length;

                for (int i = 0; i < length; i++)
                {
                    var targetEntity = this.TargetEntities[i];
                    if (targetEntity == Entity.Null) continue;

                    this.TargetInfoMap.Add(this.MainEntityArray[i], new()
                    {
                        TargetEntity = targetEntity,
                        Position = this.TargetPositions[i],
                    });
                }

            }

        }

    }

}

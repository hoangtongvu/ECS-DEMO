using Components.GameEntity;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
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

namespace Systems.Simulation.UnitAndBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuilderFindConstructionSystemGroup : ComponentSystemGroup
    {
        public BuilderFindConstructionSystemGroup()
        {
            this.RateManager = new RateUtils.VariableRateManager(2000);
        }
    }

    [UpdateInGroup(typeof(BuilderFindConstructionSystemGroup))]
    [BurstCompile]
    public partial struct BuilderFindConstructionSystem : ISystem
    {
        private EntityQuery needConstructionBuildingQuery;
        private EntityQuery builderUnitQuery;
        private EntityQuery setCanOverrideMoveCommandTagJobQuery;
        private EntityQuery setTargetJobQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.needConstructionBuildingQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , FactionIndex
                    , ConstructionRemaining>()
                .Build();

            this.builderUnitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , FactionIndex
                    , InteractingEntity
                    , MoveCommandElement
                    , IsBuilderUnitTag>()
                .WithPresent<
                    CanSetTargetJobScheduleTag>()
                .Build();

            this.setCanOverrideMoveCommandTagJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , IsBuilderUnitTag>()
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
                    , IsBuilderUnitTag>()
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

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>();
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;

            var buildingEntities = this.needConstructionBuildingQuery.ToEntityArray(Allocator.TempJob);
            var buildingTransforms = this.needConstructionBuildingQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            int buildingCount = buildingEntities.Length;

            var builderEntities = this.builderUnitQuery.ToEntityArray(Allocator.TempJob);
            int builderUnitCount = builderEntities.Length;

            var targetEntities = new NativeArray<Entity>(builderUnitCount, Allocator.TempJob);
            var targetPositions = new NativeArray<float3>(builderUnitCount, Allocator.TempJob);
            var targetInfoMap = new NativeHashMap<Entity, TargetEntityInfo>(builderUnitCount, Allocator.TempJob);

            state.Dependency = new GetTargetEntitiesAndPositionsJob
            {
                BuildingCount = buildingCount,
                BuildingEntities = buildingEntities,
                BuildingTransforms = buildingTransforms,
                FactionIndexLookup = SystemAPI.GetComponentLookup<FactionIndex>(),
                TargetEntities = targetEntities,
                TargetPositions = targetPositions,
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new InitTargetInfoMapJob
            {
                MainEntityArray = builderEntities,
                TargetEntities = targetEntities,
                TargetPositions = targetPositions,
                TargetInfoMap = targetInfoMap,
            }.Schedule(state.Dependency);

            state.Dependency = new SetCanOverrideMoveCommandTagJob
            {
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                NewMoveCommandSource = MoveCommandSource.AutoAttack, // TODO: Change this to something else
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

            state.Dependency = buildingEntities.Dispose(state.Dependency);
            state.Dependency = buildingTransforms.Dispose(state.Dependency);
            state.Dependency = builderEntities.Dispose(state.Dependency);
            state.Dependency = targetEntities.Dispose(state.Dependency);
            state.Dependency = targetPositions.Dispose(state.Dependency);
            state.Dependency = targetInfoMap.Dispose(state.Dependency);

        }

        [WithAll(typeof(IsBuilderUnitTag))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct GetTargetEntitiesAndPositionsJob : IJobEntity
        {
            [ReadOnly] public int BuildingCount;
            [ReadOnly] public NativeArray<Entity> BuildingEntities;
            [ReadOnly] public NativeArray<LocalTransform> BuildingTransforms;
            [ReadOnly] public ComponentLookup<FactionIndex> FactionIndexLookup;

            [WriteOnly] public NativeArray<Entity> TargetEntities;
            [WriteOnly] public NativeArray<float3> TargetPositions;

            void Execute(
                in LocalTransform unitTransform
                , in FactionIndex unitFactionIndex
                , EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
                , in InteractingEntity interactingEntity
                , in MoveCommandElement moveCommandElement
                , [EntityIndexInQuery] int entityIndex)
            {
                if (interactingEntity.Value != Entity.Null) return;
                if (moveCommandElement.TargetEntity != Entity.Null) return;

                bool canGetBuildingIndex =
                    this.TryGetNearestValidBuildingIndex(in unitTransform.Position, in unitFactionIndex.Value, out int buildingIndex);

                if (!canGetBuildingIndex) return;

                this.TargetEntities[entityIndex] = this.BuildingEntities[buildingIndex];
                this.TargetPositions[entityIndex] = this.BuildingTransforms[buildingIndex].Position;
                canSetTargetJobScheduleTag.ValueRW = true;

            }

            [BurstCompile]
            private bool TryGetNearestValidBuildingIndex(
                in float3 unitPos
                , in byte unitFactionIndex
                , out int index)
            {
                const int radiusSq = 225; // TODO: GET THIS VALUE FROM ELSE WHERE
                index = -1;
                float minDistanceSq = float.MaxValue;

                for (int i = 0; i < this.BuildingCount; i++)
                {
                    byte buildingFactionIndex = this.FactionIndexLookup.GetRefRO(this.BuildingEntities[i]).ValueRO.Value;
                    if (unitFactionIndex != buildingFactionIndex) continue;

                    float distanceSq = math.distancesq(this.BuildingTransforms[i].Position, unitPos);
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

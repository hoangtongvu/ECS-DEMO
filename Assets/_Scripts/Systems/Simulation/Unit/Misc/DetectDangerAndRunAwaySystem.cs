using Components.GameEntity;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Core.GameEntity.Movement.MoveCommand;
using Core.Misc;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Utilities;
using Utilities.Jobs;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct DetectDangerAndRunAwaySystem : ISystem
    {
        private EntityQuery entityQuery;
        private EntityQuery setCanOverrideMoveCommandTagJobQuery;
        private EntityQuery setTargetJobQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.CreateSystemTimer(ref state);

            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , LocalTransform
                    , MoveSpeedLinear>()
                .WithAll<
                    MoveCommandElement
                    , InteractingEntity
                    , FactionIndex
                    , CanSetTargetJobScheduleTag>()
                .WithAll<
                    IsUnarmedEntityTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build();

            this.setCanOverrideMoveCommandTagJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder>()
                .WithAll<
                    MoveCommandElement
                    , InteractingEntity
                    , InteractionTypeICD
                    , ArmedStateHolder>()
                .WithAll<
                    IsUnarmedEntityTag>()
                .WithAll<
                    CanSetTargetJobScheduleTag>()
                .WithPresent<
                    CanOverrideMoveCommandTag>()
                .Build();

            this.setTargetJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder>()
                .WithAll<
                    LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , MoveSpeedLinear
                    , TargetEntity
                    , TargetEntityWorldSquareRadius
                    , MoveCommandElement
                    , InteractableDistanceRange>()
                .WithAll<
                    IsUnarmedEntityTag>()
                .WithAll<
                    CanSetTargetJobScheduleTag
                    , CanOverrideMoveCommandTag>()
                .WithPresent<
                    CanFindPathTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
            state.RequireForUpdate<MoveCommandPrioritiesMap>();
            state.RequireForUpdate<UnitReactionConfigsMap>();
            state.RequireForUpdate<DefaultStopMoveWorldRadius>();
            state.RequireForUpdate<DetectionRadiusMap>();
            state.RequireForUpdate<UnarmedUnitFleeTotalSeconds>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var timerRef = SystemAPI.GetSingletonRW<DetectDangerAndRunAwaySystemTimer>();
            timerRef.ValueRW.TimeCounterSeconds += new half(SystemAPI.Time.DeltaTime);

            if (timerRef.ValueRO.TimeCounterSeconds < timerRef.ValueRO.TimeLimitSeconds) return;
            timerRef.ValueRW.TimeCounterSeconds = half.zero;

            var detectionRadiusMap = SystemAPI.GetSingleton<DetectionRadiusMap>();
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var fleeTotalSeconds = SystemAPI.GetSingleton<UnarmedUnitFleeTotalSeconds>().Value;

            int entityCount = this.entityQuery.CalculateEntityCount();
            var targetInfoMap = new NativeHashMap<Entity, TargetEntityInfo>(entityCount, Allocator.TempJob);

            var mainEntityArray = this.entityQuery.ToEntityArray(Allocator.TempJob);
            var dangerousEntityArray = new NativeArray<Entity>(entityCount, Allocator.TempJob);
            var dangerousEntityPosArray = new NativeArray<float3>(entityCount, Allocator.TempJob);
            var runawayDestinationArray = new NativeArray<float3>(entityCount, Allocator.TempJob);

            state.Dependency = new GetTargetEntitiesAndPositionsJob
            {
                PhysicsWorld = physicsWorld,
                DetectionRadiusMap = detectionRadiusMap,
                FactionIndexLookup = SystemAPI.GetComponentLookup<FactionIndex>(),
                IsArmedEntityTagLookup = SystemAPI.GetComponentLookup<IsArmedEntityTag>(),
                DangerousEntityArray = dangerousEntityArray,
                DangerousEntityPosArray = dangerousEntityPosArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new InitTargetInfoMapJob
            {
                MainEntityArray = mainEntityArray,
                TargetEntityArray = dangerousEntityArray,
                TargetPosArray = dangerousEntityPosArray,
                TargetInfoMap = targetInfoMap,
            }.Schedule(state.Dependency);

            state.Dependency = new SetCanOverrideMoveCommandTagJob
            {
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                NewMoveCommandSource = MoveCommandSource.Danger,
            }.ScheduleParallel(this.setCanOverrideMoveCommandTagJobQuery, state.Dependency);

            state.Dependency = new SetSpeedsAsRunSpeedsJob
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new GetRunawayDestinationsJob
            {
                FleeTotalSeconds = fleeTotalSeconds,
                TargetInfoMap = targetInfoMap,
                RunawayDestinationArray = runawayDestinationArray,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new SetTargetPositionsJob
            {
                TargetPositions = runawayDestinationArray,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new CleanTagsJob().ScheduleParallel(state.Dependency);

            state.Dependency = targetInfoMap.Dispose(state.Dependency);
            state.Dependency = mainEntityArray.Dispose(state.Dependency);
            state.Dependency = dangerousEntityArray.Dispose(state.Dependency);
            state.Dependency = dangerousEntityPosArray.Dispose(state.Dependency);
            state.Dependency = runawayDestinationArray.Dispose(state.Dependency);

        }

        [BurstCompile]
        private void CreateSystemTimer(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new DetectDangerAndRunAwaySystemTimer
            {
                TimeCounterSeconds = half.zero,
                TimeLimitSeconds = new(2f),
            });

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [WithAll(typeof(IsArmedEntityTag))]
        [BurstCompile]
        private partial struct GetTargetEntitiesAndPositionsJob : IJobEntity
        {
            [ReadOnly] public PhysicsWorldSingleton PhysicsWorld;
            [ReadOnly] public DetectionRadiusMap DetectionRadiusMap;
            [ReadOnly] public ComponentLookup<FactionIndex> FactionIndexLookup;
            [ReadOnly] public ComponentLookup<IsArmedEntityTag> IsArmedEntityTagLookup;

            public NativeArray<Entity> DangerousEntityArray;
            public NativeArray<float3> DangerousEntityPosArray;

            [BurstCompile]
            void Execute(
                in UnitProfileIdHolder unitProfileIdHolder
                , in LocalTransform transform
                , in FactionIndex factionIndex
                , EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
                , Entity unitEntity
                , [EntityIndexInQuery] int entityIndex)
            {
                var hitList = new NativeList<DistanceHit>(Allocator.Temp);

                half detectionRadius = this.DetectionRadiusMap.Value[unitProfileIdHolder.Value];

                bool hasHit = this.PhysicsWorld.OverlapSphere(
                    transform.Position
                    , detectionRadius
                    , ref hitList
                    , new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayer.Unit,
                        CollidesWith = (uint)(CollisionLayer.Unit | CollisionLayer.Player),
                    });

                if (!hasHit)
                {
                    hitList.Dispose();
                    return;
                }

                int length = hitList.Length;
                var dangerousEntity = Entity.Null;
                float3 dangerousEntityPosition = float3.zero;
                float smallestDistanceXZ = float.MaxValue;

                for (int i = 0; i < length; i++)
                {
                    var hit = hitList[i];

                    // Note: The following logic works fine, just commented this out for testing purpose
                    //this.FactionIndexLookup.TryGetComponent(hit.Entity, out var targetFactionIndex);
                    //if (targetFactionIndex == FactionIndex.Neutral) continue;
                    //if (factionIndex.Value == targetFactionIndex.Value) continue;

                    bool isDangerousEntity = this.IsArmedEntityTagLookup.HasComponent(hit.Entity);
                    if (!isDangerousEntity) continue;

                    if (hit.Entity == unitEntity) continue;
                    if (smallestDistanceXZ <= hit.Distance) continue;

                    smallestDistanceXZ = hit.Distance;
                    dangerousEntity = hit.Entity;
                    dangerousEntityPosition = hit.Position;
                }

                this.DangerousEntityArray[entityIndex] = dangerousEntity;
                this.DangerousEntityPosArray[entityIndex] = dangerousEntityPosition;

                if (dangerousEntity != Entity.Null)
                    canSetTargetJobScheduleTag.ValueRW = true;

                hitList.Dispose();
            }

        }

        [BurstCompile]
        private partial struct InitTargetInfoMapJob : IJob
        {
            [ReadOnly] public NativeArray<Entity> MainEntityArray;
            [ReadOnly] public NativeArray<Entity> TargetEntityArray;
            [ReadOnly] public NativeArray<float3> TargetPosArray;

            public NativeHashMap<Entity, TargetEntityInfo> TargetInfoMap;

            [BurstCompile]
            public void Execute()
            {
                int length = MainEntityArray.Length;

                for (int i = 0; i < length; i++)
                {
                    var targetEntity = this.TargetEntityArray[i];
                    if (targetEntity == Entity.Null) continue;

                    this.TargetInfoMap.Add(this.MainEntityArray[i], new()
                    {
                        TargetEntity = targetEntity,
                        Position = this.TargetPosArray[i],
                    });
                }

            }

        }

        [WithAll(typeof(CanSetTargetJobScheduleTag))]
        [WithAll(typeof(CanOverrideMoveCommandTag))]
        [BurstCompile]
        private partial struct GetRunawayDestinationsJob : IJobEntity
        {
            [ReadOnly] public half FleeTotalSeconds;
            [ReadOnly] public NativeHashMap<Entity, TargetEntityInfo> TargetInfoMap;
            [WriteOnly] public NativeArray<float3> RunawayDestinationArray;

            [BurstCompile]
            void Execute(
                in LocalTransform transform
                , in MoveSpeedLinear moveSpeedLinear
                , Entity entity
                , [EntityIndexInQuery] int entityIndex)
            {
                this.RunawayDestinationArray[entityIndex] = this.GetRunAwayDestination(
                    in transform.Position
                    , this.TargetInfoMap[entity].Position
                    , in moveSpeedLinear.Value
                    , in this.FleeTotalSeconds);

            }

            [BurstCompile]
            private float3 GetRunAwayDestination(
                in float3 unitPos
                , in float3 dangerousPos
                , in float unitSpeed
                , in half totalRunAwaySeconds)
            {
                float3 runDir = math.normalize(unitPos - dangerousPos).With(y: 0f);
                float3 distanceVector = totalRunAwaySeconds * unitSpeed * runDir;
                return distanceVector + unitPos;
            }

        }

    }

}
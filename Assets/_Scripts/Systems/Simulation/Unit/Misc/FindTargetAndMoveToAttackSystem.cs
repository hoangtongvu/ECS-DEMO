using Components.GameEntity;
using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Core.GameEntity.Movement.MoveCommand;
using Core.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Utilities.Jobs;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct FindTargetAndMoveToAttackSystem : ISystem
    {
        private EntityQuery entityQuery;
        private EntityQuery setCanOverrideMoveCommandTagJobQuery;
        private EntityQuery setTargetJobQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
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
                    IsArmedEntityTag>()
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
                    IsArmedEntityTag>()
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
                    IsArmedEntityTag>()
                .WithAll<
                    CanSetTargetJobScheduleTag
                    , CanOverrideMoveCommandTag>()
                .WithPresent<
                    CanFindPathTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
            state.RequireForUpdate<MoveCommandPrioritiesMap>();
            state.RequireForUpdate<UnitReactionConfigsMap>();
            state.RequireForUpdate<AttackConfigsMap>();
            state.RequireForUpdate<DetectionRadiusMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>();
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            var attackConfigsMap = SystemAPI.GetSingleton<AttackConfigsMap>();
            var detectionRadiusMap = SystemAPI.GetSingleton<DetectionRadiusMap>();
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            int entityCount = this.entityQuery.CalculateEntityCount();
            var targetInfoMap = new NativeHashMap<Entity, TargetEntityInfo>(entityCount, Allocator.TempJob);

            var mainEntityArray = this.entityQuery.ToEntityArray(Allocator.TempJob);
            var targetEntityArray = new NativeArray<Entity>(entityCount, Allocator.TempJob);
            var targetPosArray = new NativeArray<float3>(entityCount, Allocator.TempJob);

            state.Dependency = new GetTargetEntitiesAndPositionsJob
            {
                PhysicsWorld = physicsWorld,
                DetectionRadiusMap = detectionRadiusMap,
                FactionIndexLookup = SystemAPI.GetComponentLookup<FactionIndex>(),
                IsAliveTagLookup = SystemAPI.GetComponentLookup<IsAliveTag>(),
                TargetEntityArray = targetEntityArray,
                TargetPosArray = targetPosArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new InitTargetInfoMapJob
            {
                TargetInfoMap = targetInfoMap,
                MainEntityArray = mainEntityArray,
                TargetEntityArray = targetEntityArray,
                TargetPosArray = targetPosArray,
            }.Schedule(state.Dependency);

            state.Dependency = new SetCanOverrideMoveCommandTagJob
            {
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                NewMoveCommandSource = MoveCommandSource.AutoAttack,
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

            state.Dependency = new Set_InteractableDistanceRanges_From_AttackConfigsMap_Job
            {
                AttackConfigsMap = attackConfigsMap,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new CleanTagsJob().ScheduleParallel(state.Dependency);

            state.Dependency = targetInfoMap.Dispose(state.Dependency);
            state.Dependency = mainEntityArray.Dispose(state.Dependency);
            state.Dependency = targetEntityArray.Dispose(state.Dependency);
            state.Dependency = targetPosArray.Dispose(state.Dependency);

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [WithAll(typeof(IsArmedEntityTag))]
        [BurstCompile]
        private partial struct GetTargetEntitiesAndPositionsJob : IJobEntity
        {
            [ReadOnly]
            public PhysicsWorldSingleton PhysicsWorld;

            [ReadOnly]
            public DetectionRadiusMap DetectionRadiusMap;

            [ReadOnly]
            public ComponentLookup<FactionIndex> FactionIndexLookup;

            [ReadOnly]
            public ComponentLookup<IsAliveTag> IsAliveTagLookup;

            public NativeArray<Entity> TargetEntityArray;
            public NativeArray<float3> TargetPosArray;

            [BurstCompile]
            void Execute(
                in UnitProfileIdHolder unitProfileIdHolder
                , in MoveCommandElement moveCommandElement
                , in InteractingEntity interactingEntity
                , in LocalTransform transform
                , in FactionIndex factionIndex
                , EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
                , Entity unitEntity
                , [EntityIndexInQuery] int entityIndex)
            {
                if (interactingEntity.Value != Entity.Null) return;
                if (moveCommandElement.TargetEntity != Entity.Null) return;

                var hitList = new NativeList<DistanceHit>(Allocator.Temp);

                half detectionRadius = this.DetectionRadiusMap.Value[unitProfileIdHolder.Value];

                bool hasHit = this.PhysicsWorld.OverlapSphere(
                    transform.Position
                    , detectionRadius
                    , ref hitList
                    , new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayer.Unit,
                        CollidesWith = (uint)(CollisionLayerConstants.Damagable),
                    });

                if (!hasHit) return;

                int length = hitList.Length;
                var targetEntity = Entity.Null;
                float3 targetPosition = float3.zero;
                float smallestDistanceXZ = float.MaxValue;

                for (int i = 0; i < length; i++)
                {
                    var hit = hitList[i];

                    if (!this.IsAliveTagLookup.HasComponent(hit.Entity)) continue;
                    if (!this.IsAliveTagLookup.IsComponentEnabled(hit.Entity)) continue;

                    if (!this.FactionIndexLookup.TryGetComponent(hit.Entity, out var targetFactionIndex)) continue;
                    if (targetFactionIndex == FactionIndex.Neutral) continue;
                    if (factionIndex.Value == targetFactionIndex.Value) continue;

                    if (hit.Entity == unitEntity) continue;
                    if (smallestDistanceXZ <= hit.Distance) continue;

                    smallestDistanceXZ = hit.Distance;
                    targetEntity = hit.Entity;
                    targetPosition = hit.Position;
                }

                this.TargetEntityArray[entityIndex] = targetEntity;
                this.TargetPosArray[entityIndex] = targetPosition;

                if (targetEntity != Entity.Null)
                    canSetTargetJobScheduleTag.ValueRW = true;

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

        [WithAll(typeof(IsArmedEntityTag))]
        [WithAll(typeof(CanSetTargetJobScheduleTag))]
        [BurstCompile]
        private partial struct Set_InteractableDistanceRanges_From_AttackConfigsMap_Job : IJobEntity
        {
            [ReadOnly] public AttackConfigsMap AttackConfigsMap;

            [BurstCompile]
            void Execute(
                in UnitProfileIdHolder unitProfileIdHolder
                , ref InteractableDistanceRange interactableDistanceRange)
            {
                interactableDistanceRange.MinValue = this.AttackConfigsMap.Value[unitProfileIdHolder.Value].MinAttackDistance;
                interactableDistanceRange.MaxValue = this.AttackConfigsMap.Value[unitProfileIdHolder.Value].MaxAttackDistance;

            }

        }

    }

}
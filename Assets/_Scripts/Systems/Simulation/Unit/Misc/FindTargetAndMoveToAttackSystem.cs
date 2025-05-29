using Components;
using Components.GameEntity;
using Components.Misc;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.MyMoveCommand;
using Components.Unit.Reaction;
using Components.Unit.UnitSelection;
using Core;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
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

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , UnitSelectedTag
                    , CanFindPathTag
                    , MoveSpeedLinear
                    , TargetEntity>()
                .WithAll<
                    TargetEntityWorldSquareRadius
                    , MoveCommandElement
                    , InteractingEntity
                    , InteractionTypeICD
                    , ArmedStateHolder
                    , AutoAttackDetectionRadius
                    , CanSetTargetJobScheduleTag>()
                .WithAll<
                    IsArmedUnitTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build();

            state.RequireForUpdate(this.entityQuery);
            state.RequireForUpdate<MoveCommandPrioritiesMap>();
            state.RequireForUpdate<UnitReactionConfigsMap>();
            state.RequireForUpdate<DefaultStopMoveWorldRadius>();
            state.RequireForUpdate<IsUnarmedUnitTag>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;
            var defaultStopMoveWorldRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            int entityCount = this.entityQuery.CalculateEntityCount();
            var speedArray = new NativeArray<float>(entityCount, Allocator.TempJob);
            var attackTargetArray = new NativeArray<Entity>(entityCount, Allocator.TempJob);
            var targetPosArray = new NativeArray<float3>(entityCount, Allocator.TempJob);

            state.Dependency = new GetTargetEntitiesAndPositionsJob
            {
                PhysicsWorld = physicsWorld,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                TargetEntityArray = attackTargetArray,
                TargetPosArray = targetPosArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new GetRunSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
                OutputArray = speedArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new SetMultipleTargetsJobMultipleSpeeds
            {
                TargetEntities = attackTargetArray,
                TargetPositions = targetPosArray,
                TargetEntityWorldSquareRadius = defaultStopMoveWorldRadius,
                NewMoveCommandSource = MoveCommandSource.ToolCall,
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                SpeedArray = speedArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = speedArray.Dispose(state.Dependency);
            state.Dependency = attackTargetArray.Dispose(state.Dependency);
            state.Dependency = targetPosArray.Dispose(state.Dependency);

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [WithAll(typeof(IsArmedUnitTag))]
        [BurstCompile]
        private partial struct GetTargetEntitiesAndPositionsJob : IJobEntity
        {
            [ReadOnly]
            public PhysicsWorldSingleton PhysicsWorld;

            [ReadOnly]
            public ComponentLookup<LocalTransform> TransformLookup;

            public NativeArray<Entity> TargetEntityArray;
            public NativeArray<float3> TargetPosArray;

            [BurstCompile]
            void Execute(
                in MoveCommandElement moveCommandElement
                , in InteractingEntity interactingEntity
                , in AutoAttackDetectionRadius autoAttackDetectionRadius
                , in LocalTransform transform
                , EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
                , Entity unitEntity
                , [EntityIndexInQuery] int entityIndex)
            {
                if (interactingEntity.Value != Entity.Null) return;
                if (moveCommandElement.TargetEntity != Entity.Null) return;
                UnityEngine.Debug.Log("Finding Target");

                var hitList = new NativeList<DistanceHit>(Allocator.Temp);
                half detectionRadius = autoAttackDetectionRadius.Value;

                bool hasHit = this.PhysicsWorld.OverlapSphere(
                    transform.Position
                    , detectionRadius
                    , ref hitList
                    , new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayer.Unit,
                        CollidesWith = (uint)CollisionLayer.Unit,
                    });

                if (!hasHit)
                {
                    hitList.Dispose();
                    return;
                }

                int length = hitList.Length;
                var targetEntity = Entity.Null;

                for (int i = 0; i < length; i++)
                {
                    var hitEntity = hitList[i].Entity;
                    if (hitEntity == unitEntity) continue;

                    targetEntity = hitEntity;
                    break;
                }

                this.TransformLookup.TryGetComponent(targetEntity, out var targetTransform);
                this.TargetEntityArray[entityIndex] = targetEntity;
                this.TargetPosArray[entityIndex] = targetTransform.Position;

                if (targetEntity != Entity.Null)
                    canSetTargetJobScheduleTag.ValueRW = true;

                hitList.Dispose();
            }

        }

    }

}
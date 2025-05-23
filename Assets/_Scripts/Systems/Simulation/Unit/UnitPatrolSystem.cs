using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Utilities;
using Components.Unit.Reaction;
using Core.Unit.Reaction;
using Components.Misc.WorldMap.PathFinding;
using Components.GameEntity;
using Core.Unit;
using System.Collections.Generic;
using Utilities.Helpers.Misc;
using Components.Unit.Misc;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitPatrolSystem : ISystem
    {
        private Random rand;
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new(1);
            this.CreatePatrolRandomValuesMap(ref state);

            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<NeedInitWalkTag>()
                .WithAll<
                    UnitProfileIdHolder
                    , MoveCommandElement
                    , InteractingEntity
                    , InteractionTypeICD
                    , LocalTransform
                    , MoveSpeedLinear
                    , CanFindPathTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAliveTag
                    , IsUnitWorkingTag
                    , UnitIdleTimeCounter
                    , NeedInitWalkTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<MoveCommandPrioritiesMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var randomValuesMap = SystemAPI.GetSingleton<PatrolRandomValuesMap>();
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;

            randomValuesMap.Value.Clear();

            foreach (var (unitProfileIdHolderRef, idleTimeCounterRef, needInitWalkTag, entity) in
                SystemAPI.Query<
                    RefRO<UnitProfileIdHolder>
                    , RefRW<UnitIdleTimeCounter>
                    , EnabledRefRW<NeedInitWalkTag>>()
                    .WithAll<IsAliveTag>()
                    .WithDisabled<IsUnitWorkingTag>()
                    .WithEntityAccess()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!unitReactionConfigsMap.TryGetValue(unitProfileIdHolderRef.ValueRO.Value, out var unitReactionConfigs))
                    throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolderRef.ValueRO.Value}");

                bool idleTimeExceeded = idleTimeCounterRef.ValueRO.Value >= unitReactionConfigs.UnitIdleMaxDuration;
                if (!idleTimeExceeded) continue;

                idleTimeCounterRef.ValueRW.Value = 0;
                needInitWalkTag.ValueRW = true;

                this.AddRandomValuesIntoMap(
                    in randomValuesMap
                    , in entity
                    , unitReactionConfigs.UnitWalkMinDistance
                    , unitReactionConfigs.UnitWalkMaxDistance);

            }

            var speedArray = new NativeArray<float>(this.entityQuery.CalculateEntityCount(), Allocator.TempJob);

            var getSpeedsJobHandle = new GetWalkSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
                OutputArray = speedArray,
            }.ScheduleParallel(state.Dependency);

            state.Dependency = new SetPatrolJob
            {
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                RandomizedValueMap = randomValuesMap.Value,
                SpeedArray = speedArray,
            }.ScheduleParallel(getSpeedsJobHandle);

        }

        [BurstCompile]
        private void CreatePatrolRandomValuesMap(ref SystemState state)
        {
            var randomValuesMap = new PatrolRandomValuesMap
            {
                Value = new(30, Allocator.Persistent),
            };

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(randomValuesMap);

        }

        [BurstCompile]
        private void AddRandomValuesIntoMap(
            in PatrolRandomValuesMap randomValuesMap
            , in Entity keyEntity
            , float minWalkDistance
            , float maxWalkDistance)
        {
            float2 randomDir = this.rand.NextFloat2Direction();
            float randomDis = this.rand.NextFloat(minWalkDistance, maxWalkDistance);

            randomValuesMap.Value.Add(keyEntity, new()
            {
                RandomDir = randomDir,
                RandomDis = randomDis,
            });

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct GetWalkSpeedsJob : IJobEntity
        {
            [ReadOnly]
            public NativeHashMap<UnitProfileId, UnitReactionConfigs> UnitReactionConfigsMap;

            [NativeDisableParallelForRestriction]
            public NativeArray<float> OutputArray;

            [BurstCompile]
            void Execute(
                EnabledRefRW<NeedInitWalkTag> needInitWalkTag
                , in UnitProfileIdHolder unitProfileIdHolder
                , ref MoveCommandElement moveCommandElement
                , ref InteractingEntity interactingEntity
                , ref InteractionTypeICD interactionTypeICD
                , in LocalTransform transform
                , ref MoveSpeedLinear moveSpeed
                , EnabledRefRW<CanFindPathTag> canFindPathTag
                , [EntityIndexInQuery] int entityIndex)
            {
                if (!needInitWalkTag.ValueRO) return;

                if (!this.UnitReactionConfigsMap.TryGetValue(unitProfileIdHolder.Value, out var unitReactionConfigs))
                    throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolder.Value}");

                this.OutputArray[entityIndex] = unitReactionConfigs.UnitWalkSpeed;

            }

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct SetPatrolJob : IJobEntity
        {
            [ReadOnly] public MoveCommandPrioritiesMap MoveCommandPrioritiesMap;
            [ReadOnly] public NativeHashMap<Entity, PatrolRandomValues> RandomizedValueMap;

            [DeallocateOnJobCompletion]
            [ReadOnly]
            public NativeArray<float> SpeedArray;

            [BurstCompile]
            void Execute(
                EnabledRefRW<NeedInitWalkTag> needInitWalkTag
                , in ArmedStateHolder armedStateHolder
                , ref MoveCommandElement moveCommandElement
                , ref InteractingEntity interactingEntity
                , ref InteractionTypeICD interactionTypeICD
                , in LocalTransform transform
                , ref MoveSpeedLinear moveSpeed
                , EnabledRefRW<CanFindPathTag> canFindPathTag
                , Entity entity
                , [EntityIndexInQuery] int entityIndex)
            {
                if (!needInitWalkTag.ValueRO) return;
                needInitWalkTag.ValueRW = false;

                bool canOverrideCommand =
                    MoveCommandPrioritiesHelper.TryOverrideMoveCommand(
                        in this.MoveCommandPrioritiesMap
                        , ref moveCommandElement
                        , ref interactingEntity
                        , ref interactionTypeICD
                        , armedStateHolder.Value
                        , MoveCommandSource.PlayerCommand);

                if (!canOverrideCommand) return;

                var randomizedValue = this.RandomizedValueMap[entity];

                float2 tempVector2 = randomizedValue.RandomDir * randomizedValue.RandomDis;

                // Calculate the random target position
                float3 randomPoint = transform.Position + new float3(tempVector2.x, 0, tempVector2.y);

                // Use this randomPoint for the move command or other logic
                moveCommandElement.Float3 = randomPoint;

                // Set move speed
                moveSpeed.Value = this.SpeedArray[entityIndex];

                canFindPathTag.ValueRW = true;

            }

        }

    }

}
using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;
using Components.Misc.GlobalConfigs;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Unity.Mathematics;
using Utilities.Helpers;
using Unity.Collections;
using Components.MyEntity;
using Unity.Transforms;
using Utilities;
using Components.Unit.Reaction;
using Core.Unit.Reaction;
using Components.Misc.WorldMap.PathFinding;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitPatrolSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new(1);
            this.CreatePatrolRandomValuesMap(ref state);

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAliveTag
                    , IsUnitWorkingTag
                    , UnitIdleTimeCounter
                    , NeedInitWalkTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<MoveCommandSourceMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var randomValuesMap = SystemAPI.GetSingleton<PatrolRandomValuesMap>();

            randomValuesMap.Value.Clear();

            foreach (var (idleTimeCounterRef, needInitWalkTag, entity) in
                SystemAPI.Query<
                    RefRW<UnitIdleTimeCounter>
                    , EnabledRefRW<NeedInitWalkTag>>()
                    .WithAll<IsAliveTag>()
                    .WithDisabled<IsUnitWorkingTag>()
                    .WithEntityAccess()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool idleTimeExceeded = idleTimeCounterRef.ValueRO.Value >= gameGlobalConfigs.Value.UnitIdleMaxDuration;
                if (!idleTimeExceeded) continue;

                idleTimeCounterRef.ValueRW.Value = 0;
                needInitWalkTag.ValueRW = true;

                this.AddRandomValuesIntoMap(
                    in randomValuesMap
                    , in entity
                    , gameGlobalConfigs.Value.UnitWalkMinDistance
                    , gameGlobalConfigs.Value.UnitWalkMaxDistance);

            }

            state.Dependency = new SetPatrolJob
            {
                moveCommandSourceMap = moveCommandSourceMap.Value,
                RandomizedValueMap = randomValuesMap.Value,
                UnitWalkMinDistance = gameGlobalConfigs.Value.UnitWalkMinDistance,
                UnitWalkMaxDistance = gameGlobalConfigs.Value.UnitWalkMaxDistance,
                UnitWalkSpeed = gameGlobalConfigs.Value.UnitWalkSpeed,
            }.ScheduleParallel(state.Dependency);

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
        private partial struct SetPatrolJob : IJobEntity
        {
            [ReadOnly] public NativeHashMap<MoveCommandSourceId, byte> moveCommandSourceMap;
            [ReadOnly] public NativeHashMap<Entity, PatrolRandomValues> RandomizedValueMap;
            [ReadOnly] public float UnitWalkMinDistance; // Need to del
            [ReadOnly] public float UnitWalkMaxDistance; // Need to del
            [ReadOnly] public float UnitWalkSpeed;

            [BurstCompile]
            void Execute(
                EnabledRefRW<NeedInitWalkTag> needInitWalkTag
                , in UnitId unitId
                , ref MoveCommandElement moveCommandElement
                , ref InteractingEntity interactingEntity
                , ref InteractionTypeICD interactionTypeICD
                , in LocalTransform transform
                , ref MoveSpeedLinear moveSpeed
                , EnabledRefRW<CanFindPathTag> canFindPathTag
                , Entity entity)
            {
                if (!needInitWalkTag.ValueRO) return;
                needInitWalkTag.ValueRW = false;

                bool canOverrideCommand =
                    MoveCommandHelper.TryOverrideMoveCommand(
                        in moveCommandSourceMap
                        , unitId.UnitType
                        , ref moveCommandElement
                        , ref interactingEntity
                        , ref interactionTypeICD
                        , MoveCommandSource.PlayerCommand
                        , unitId.LocalIndex);

                if (!canOverrideCommand) return;

                var randomizedValue = this.RandomizedValueMap[entity];

                float2 tempVector2 = randomizedValue.RandomDir * randomizedValue.RandomDis;

                // Calculate the random target position
                float3 randomPoint = transform.Position + new float3(tempVector2.x, 0, tempVector2.y);

                // Use this randomPoint for the move command or other logic
                moveCommandElement.Float3 = randomPoint;

                // Set move speed
                moveSpeed.Value = this.UnitWalkSpeed;

                canFindPathTag.ValueRW = true;

            }

        }

    }

}
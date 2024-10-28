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

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAliveTag
                    , IsUnitWorkingTag
                    , UnitIdleTimeCounter
                    , AnimatorData
                    , NeedsInitWalkTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();

            foreach (var (idleTimeCounterRef, needInitWalkTag) in
                SystemAPI.Query<
                    RefRW<UnitIdleTimeCounter>
                    , EnabledRefRW<NeedsInitWalkTag>>()
                    .WithAll<IsAliveTag>()
                    .WithDisabled<IsUnitWorkingTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (idleTimeCounterRef.ValueRO.Value >= gameGlobalConfigs.Value.UnitIdleMaxDuration)
                {
                    idleTimeCounterRef.ValueRW.Value = 0;
                    needInitWalkTag.ValueRW = true;
                }

            }

            state.Dependency = new SetPatrolJob
            {
                moveCommandSourceMap = moveCommandSourceMap.Value,
                Rand = new Random(this.rand.NextUInt(1, 15)),
                UnitWalkMinDistance = gameGlobalConfigs.Value.UnitWalkMinDistance,
                UnitWalkMaxDistance = gameGlobalConfigs.Value.UnitWalkMaxDistance,
                UnitWalkSpeed = gameGlobalConfigs.Value.UnitWalkSpeed,
            }.ScheduleParallel(state.Dependency);

        }


        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct SetPatrolJob : IJobEntity
        {
            [ReadOnly] public NativeHashMap<MoveCommandSourceId, byte> moveCommandSourceMap;
            [ReadOnly] public Random Rand;
            [ReadOnly] public float UnitWalkMinDistance;
            [ReadOnly] public float UnitWalkMaxDistance;
            [ReadOnly] public float UnitWalkSpeed;


            [BurstCompile]
            void Execute(
                EnabledRefRW<NeedsInitWalkTag> needInitWalkTag
                , in UnitId unitId
                , ref MoveCommandElement moveCommandElement
                , ref InteractingEntity interactingEntity
                , ref InteractionTypeICD interactionTypeICD
                , in LocalTransform transform
                , ref MoveSpeedLinear moveSpeed
                , ref TargetPosition targetPosition
                , EnabledRefRW<TargetPosChangedTag> targetPosChangedTag
                , EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , ref AnimatorData animatorData)
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

                // Get a random direction
                float2 randomDir = this.Rand.NextFloat2Direction();

                // Generate a random distance between the min and max distance
                float randomDis = this.Rand.NextFloat(this.UnitWalkMinDistance, this.UnitWalkMaxDistance);

                float2 tempVector2 = randomDir * randomDis;

                // Calculate the random target position
                float3 randomPoint = transform.Position + new float3(tempVector2.x, 0, tempVector2.y);

                // Use this randomPoint for the move command or other logic
                moveCommandElement.Float3 = randomPoint;

                // Set move speed
                moveSpeed.Value = this.UnitWalkSpeed;

                targetPosition.Value = randomPoint;
                targetPosChangedTag.ValueRW = true;

                canMoveEntityTag.ValueRW = true;

                animatorData.Value.Value = "Walking_A";
                animatorData.Value.ValueChanged = true;

            }

        }


    }
}
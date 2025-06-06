using Unity.Entities;
using Unity.Burst;
using Components.Unit;
using Utilities.Extensions;
using Components.Unit.Reaction;
using Components.GameEntity.Movement;
using Components.GameEntity.Damage;
using Components.Misc;

namespace Systems.Simulation.Unit.Reaction
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial struct IdleReactionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IdleStartedTag
                    , IsAliveTag
                    , CanMoveEntityTag
                    , IsUnitWorkingTag
                    , UnitIdleTimeCounter
                    , AnimatorData>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (idleStartedTag, isAliveTag, canMoveEntityTag, isUnitWorkingTag, timeCounterRef, animatorDataRef) in
                SystemAPI.Query<
                    EnabledRefRW<IdleStartedTag>
                    , EnabledRefRO<IsAliveTag>
                    , EnabledRefRO<CanMoveEntityTag>
                    , EnabledRefRO<IsUnitWorkingTag>
                    , RefRW<UnitIdleTimeCounter>
                    , RefRW<AnimatorData>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool reactionStarted = idleStartedTag.ValueRO;
                bool canUpdate = isAliveTag.ValueRO && !canMoveEntityTag.ValueRO && !isUnitWorkingTag.ValueRO;

                if (canUpdate)
                {
                    if (!reactionStarted)
                    {
                        //OnReactionStart();
                        animatorDataRef.ValueRW.Value.ChangeValue("Idle");
                        timeCounterRef.ValueRW.Value = 0;
                        idleStartedTag.ValueRW = true;
                    }

                    //OnReactionUpdate();
                    timeCounterRef.ValueRW.Value += SystemAPI.Time.DeltaTime;

                }

                if (!canUpdate && reactionStarted)
                {
                    //OnReactionEnd();
                    idleStartedTag.ValueRW = false;
                }

            }

        }

    }

}
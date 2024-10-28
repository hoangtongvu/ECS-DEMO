using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;
using Components.Misc.GlobalConfigs;
using Components.Unit.Reaction;
using Utilities.Extensions;

namespace Systems.Simulation.Unit.Reaction
{

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial struct WalkReactionSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameGlobalConfigsICD>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    WalkStartedTag
                    , IsAliveTag
                    , IsUnitWorkingTag
                    , CanMoveEntityTag
                    , MoveSpeedLinear
                    , AnimatorData>()
                .Build();

            state.RequireForUpdate(query);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();

            foreach (var (walkStartedTag, isAliveTag, isUnitWorkingTag, canMoveEntityTag, moveSpeedLinearRef, animatorDataRef) in
                SystemAPI.Query<
                    EnabledRefRW <WalkStartedTag>
                    , EnabledRefRO<IsAliveTag>
                    , EnabledRefRO<IsUnitWorkingTag>
                    , EnabledRefRO<CanMoveEntityTag>
                    , RefRO<MoveSpeedLinear>
                    , RefRW<AnimatorData>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool currentSpeedIsWalkSpeed = moveSpeedLinearRef.ValueRO.Value == gameGlobalConfigs.Value.UnitWalkSpeed;
                bool canUpdate = isAliveTag.ValueRO && !isUnitWorkingTag.ValueRO && canMoveEntityTag.ValueRO && currentSpeedIsWalkSpeed;
                bool reactionStarted = walkStartedTag.ValueRO;

                if (canUpdate)
                {
                    if (!reactionStarted)
                    {
                        //OnReactionStart();
                        animatorDataRef.ValueRW.Value.ChangeValue("Walking_A");
                        walkStartedTag.ValueRW = true;
                    }

                    //OnReactionUpdate();

                }

                if (!canUpdate && reactionStarted)
                {
                    //OnReactionEnd();
                    walkStartedTag.ValueRW = false;
                }

            }

        }




    }
}
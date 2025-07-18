using Components.GameEntity.Damage;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Player;
using Components.Player.Misc;
using Systems.Simulation.GameEntity.Reaction.CanUpdateConditionsHandler;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems.Simulation.Player.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [BurstCompile]
    public partial struct RunCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    WalkReaction.CanUpdateTag>()
                .WithAll<
                    IsAliveTag
                    , CanMoveEntityTag
                    , CurrentMoveSpeed
                    , PlayerReactionConfigsHolder>()
                .WithAll<
                    PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool playerInput = Input.GetKey(KeyCode.LeftShift);

            foreach (var (reactionCanUpdateTag, isAliveTag, canMoveEntityTag) in SystemAPI
                .Query<
                    EnabledRefRW<RunReaction.CanUpdateTag>
                    , EnabledRefRO<IsAliveTag>
                    , EnabledRefRO<CanMoveEntityTag>>()
                .WithAll<
                    PlayerTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                reactionCanUpdateTag.ValueRW = isAliveTag.ValueRO && canMoveEntityTag.ValueRO && playerInput;
            }

        }

    }

}
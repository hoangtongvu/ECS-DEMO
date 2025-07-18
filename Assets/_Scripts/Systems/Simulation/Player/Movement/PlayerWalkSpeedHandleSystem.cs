using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Player;
using Components.Player.Misc;
using Systems.Simulation.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Player.Movement
{
    [UpdateInGroup(typeof(SetTargetMoveSpeedSystemGroup))]
    [BurstCompile]
    public partial struct PlayerWalkSpeedHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , TargetMoveSpeed
                    , PlayerReactionConfigsHolder
                    , WalkReaction.UpdatingTag>()
                .WithAll<
                    PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetTargetMoveSpeedJob().ScheduleParallel();
        }

        [WithAll(typeof(PlayerTag))]
        [WithAll(typeof(CanMoveEntityTag))]
        [WithAll(typeof(WalkReaction.UpdatingTag))]
        [BurstCompile]
        private partial struct SetTargetMoveSpeedJob : IJobEntity
        {
            private void Execute(
                ref TargetMoveSpeed targetMoveSpeed
                , PlayerReactionConfigsHolder playerReactionConfigsHolder)
            {
                targetMoveSpeed.Value = playerReactionConfigsHolder.Value.WalkSpeed;
            }

        }

    }

}
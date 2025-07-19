using Components.GameEntity.Reaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.Reaction.ReactionsHandler
{
    [UpdateInGroup(typeof(ReactionsHandleSystemGroup))]
    [BurstCompile]
    public partial struct WalkReactionTagsHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    WalkReaction.StartedTag
                    , WalkReaction.CanUpdateTag
                    , WalkReaction.UpdatingTag
                    , WalkReaction.EndedTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new ReactionTagsSetJob().ScheduleParallel(state.Dependency);
        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct ReactionTagsSetJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                EnabledRefRW<WalkReaction.StartedTag> reactionStartedTag
                , EnabledRefRO<WalkReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRW<WalkReaction.UpdatingTag> reactionUpdatingTag
                , EnabledRefRW<WalkReaction.EndedTag> reactionEndedTag)
            {
                reactionStartedTag.ValueRW = false;
                reactionEndedTag.ValueRW = false;

                if (reactionCanUpdateTag.ValueRO)
                {
                    if (!reactionStartedTag.ValueRO && !reactionUpdatingTag.ValueRO)
                    {
                        //OnReactionStart();
                        reactionStartedTag.ValueRW = true;
                        reactionUpdatingTag.ValueRW = true;
                    }

                    //OnReactionUpdate();
                }
                else
                {
                    if (reactionUpdatingTag.ValueRO && !reactionEndedTag.ValueRO)
                    {
                        //OnReactionEnd();
                        reactionEndedTag.ValueRW = true;
                        reactionUpdatingTag.ValueRW = false;
                    }
                }

            }

        }

    }

}
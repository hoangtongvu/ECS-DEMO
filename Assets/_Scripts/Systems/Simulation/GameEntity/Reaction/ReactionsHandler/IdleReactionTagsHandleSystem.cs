using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.Reaction;

namespace Systems.Simulation.GameEntity.Reaction.ReactionsHandler
{
    [UpdateInGroup(typeof(ReactionsHandleSystemGroup))]
    [BurstCompile]
    public partial struct IdleReactionTagsHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IdleReaction.StartedTag
                    , IdleReaction.CanUpdateTag
                    , IdleReaction.UpdatingTag
                    , IdleReaction.EndedTag>()
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
                EnabledRefRW<IdleReaction.StartedTag> reactionStartedTag
                , EnabledRefRO<IdleReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRW<IdleReaction.UpdatingTag> reactionUpdatingTag
                , EnabledRefRW<IdleReaction.EndedTag> reactionEndedTag)
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
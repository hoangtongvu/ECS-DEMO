using Unity.Entities;
using Components.GameEntity.Reaction;
using Components.Misc.Presenter;
using ZBase.Foundation.PubSub;
using Core.GameEntity.Reaction;

namespace Systems.Simulation.GameEntity.Reaction.PresenterMessagesPublisher
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackOnStartedMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    AttackReaction.StartedTag>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    AttackReaction.StartedTag>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnAttackStartedMessage());
            }

        }

    }

}
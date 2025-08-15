using Unity.Entities;
using Components.Misc.Presenter;
using ZBase.Foundation.PubSub;
using Components.Unit.Reaction;
using Core.Unit.Reaction;
using Components.Unit.Misc;

namespace Systems.Simulation.Unit.Reaction.PresenterMessagesPublisher
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class WorkOnStartedMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    WorkReaction.StartedTag>()
                .WithAll<
                    UnitTag>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    WorkReaction.StartedTag>()
                .WithAll<
                    UnitTag>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnWorkStartedMessage());
            }

        }

    }

}
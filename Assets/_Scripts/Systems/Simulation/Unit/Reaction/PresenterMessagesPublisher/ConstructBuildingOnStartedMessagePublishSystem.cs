using Unity.Entities;
using Components.Misc.Presenter;
using ZBase.Foundation.PubSub;
using Components.Unit.Misc;
using Components.GameEntity.Reaction;
using Core.GameEntity.Reaction;

namespace Systems.Simulation.Unit.Reaction.PresenterMessagesPublisher
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ConstructBuildingOnStartedMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    ConstructBuildingReaction.StartedTag>()
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
                    ConstructBuildingReaction.StartedTag>()
                .WithAll<
                    UnitTag>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnConstructBuildingStartedMessage());
            }

        }

    }

}
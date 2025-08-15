using Unity.Entities;
using Components.Misc.Presenter;
using ZBase.Foundation.PubSub;
using Components.Unit.Misc;
using Components.GameEntity.Reaction;
using Core.Harvest.Messages;
using Components.Unit;
using Core.Harvest;

namespace Systems.Simulation.Unit.Reaction.PresenterMessagesPublisher
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ChopOnStartedMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , HarvesteeTypeHolder>()
                .WithAll<
                    HarvestReaction.StartedTag>()
                .WithAll<
                    UnitTag>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (presenterHolderRef, harvesteeTypeHolderRef) in SystemAPI
                .Query<
                    RefRO<PresenterHolder>
                    , RefRO<HarvesteeTypeHolder>>()
                .WithAll<
                    HarvestReaction.StartedTag>()
                .WithAll<
                    UnitTag>())
            {
                if (harvesteeTypeHolderRef.ValueRO.Value != HarvesteeType.Tree) continue;

                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnChopStartedMessage());
            }

        }

    }

}
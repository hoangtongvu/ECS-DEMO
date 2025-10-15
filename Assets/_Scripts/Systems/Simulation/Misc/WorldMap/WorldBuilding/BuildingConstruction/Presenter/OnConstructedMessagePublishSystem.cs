using Components.Misc.Presenter;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Core.GameBuilding.BuildingConstruction.Presenter.PresenterMessages;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction.Presenter
{
    [UpdateInGroup(typeof(ConstructionOccurredEventHandleSystemGroup))]
    public partial class OnConstructedMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    ConstructionOccurredEvent>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    ConstructionOccurredEvent>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new ConstructionOccurredMessage());
            }

        }

    }

}
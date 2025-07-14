using Components.Misc.Presenter;
using Components.Misc.WorldMap.WorldBuilding;
using Core.GameBuilding.BuildingConstruction.Presenter.PresenterMessages;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction.Presenter
{
    [UpdateInGroup(typeof(EndConstructionProcessSystemGroup))]
    public partial class PresenterOnEndConstructionHandleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    ConstructionNewlyEndedTag>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    ConstructionNewlyEndedTag>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnConstructionEndedMessage());

            }

        }

    }

}
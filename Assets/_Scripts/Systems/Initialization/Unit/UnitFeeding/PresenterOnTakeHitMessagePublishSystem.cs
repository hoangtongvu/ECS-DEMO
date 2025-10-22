using Components.Misc.Presenter;
using Components.Unit.UnitFeeding;
using Core.Unit.Presenter.PresenterMessages;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PresenterOnTakeHitMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    StarvingDmgTakenEvent>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    StarvingDmgTakenEvent>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnStarvingDmgTakenMessage());
            }

        }

    }

}
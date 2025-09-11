using Components.GameEntity.Damage;
using Components.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using Systems.Initialization.GameEntity.Damage.DeadResolve;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.GameEntity.Damage.Presenter
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    public partial class PresenterOnDeadHandleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    DeadEvent>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    DeadEvent>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnDeadMessage());
            }

        }

    }

}
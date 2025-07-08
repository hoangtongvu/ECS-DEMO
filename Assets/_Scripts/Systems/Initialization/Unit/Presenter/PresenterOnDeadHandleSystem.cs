using Components.GameEntity.Damage;
using Components.Misc.Presenter;
using Components.Unit.Misc;
using Core.Misc.Presenter.PresenterMessages;
using Core.Unit.Presenter;
using Systems.Initialization.GameEntity.Damage;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.Unit.Presenter
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(HpChangesHandleSystem))]
    public partial class PresenterOnDeadHandleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder>()
                .WithAll<
                    UnitTag>()
                .WithDisabled<
                    IsAliveTag>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var presenterHolderRef in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    UnitTag>()
                .WithAll<
                    NewlyDeadTag>())
            {
                if (presenterHolderRef.ValueRO.Value.Value is not UnitPresenter unitPresenter) continue;

                unitPresenter.Messenger.MessagePublisher
                    .Publish(new OnDeadMessage());

            }

        }

    }

}
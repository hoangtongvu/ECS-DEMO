using Components.GameEntity.Damage;
using Components.GameEntity.Misc;
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
    public partial class PresenterOnTakeHitHandleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    HpChangeRecordElement
                    , PresenterHolder>()
                .WithAll<
                    UnitTag
                    , IsAliveTag>()
                .Build();

            this.RequireForUpdate(query);
            this.RequireForUpdate<FlashOnTakeHitMaterial>();
        }

        protected override void OnUpdate()
        {
            var flashOnTakeHitMaterial = SystemAPI.ManagedAPI.GetSingleton<FlashOnTakeHitMaterial>().Value;

            foreach (var (hpChangeRecords, presenterHolderRef) in SystemAPI
                .Query<
                    DynamicBuffer<HpChangeRecordElement>
                    , RefRO<PresenterHolder>>()
                .WithAll<
                    UnitTag
                    , IsAliveTag>())
            {
                if (hpChangeRecords.Length == 0) continue;
                if (presenterHolderRef.ValueRO.Value.Value is not UnitPresenter unitPresenter) continue;

                unitPresenter.Messenger.MessagePublisher
                    .Publish(new OnHitMessage(0));

            }

        }

    }

}
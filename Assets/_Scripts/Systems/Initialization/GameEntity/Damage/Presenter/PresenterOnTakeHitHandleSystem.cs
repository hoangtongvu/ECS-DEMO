using Components.GameEntity.Damage;
using Components.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.GameEntity.Damage.Presenter
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup))]
    public partial class PresenterOnTakeHitHandleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    HpChangeRecordElement
                    , PresenterHolder
                    , CurrentHp
                    , HpDataHolder>()
                .WithAll<
                    TakeHitEvent>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var (hpChangeRecords, presenterHolderRef, currentHpRef, hpDataHolder) in SystemAPI
                .Query<
                    DynamicBuffer<HpChangeRecordElement>
                    , RefRO<PresenterHolder>
                    , RefRO<CurrentHp>
                    , HpDataHolder>()
                .WithAll<
                    TakeHitEvent>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                int hitDmgValue = hpChangeRecords[0].Value;
                float remainingHpRatio = (float)currentHpRef.ValueRO.Value / hpDataHolder.Value.MaxHp;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnHitMessage(hitDmgValue, remainingHpRatio));
            }

        }

    }

}
using Components.GameEntity.Damage;
using Components.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using Systems.Initialization.GameEntity.Damage.HpChangesHandle;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.GameEntity.Damage.Presenter
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup))]
    public partial class PresenterOnHealedMessagePublishSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , CurrentHp
                    , HpDataHolder>()
                .WithAll<
                    GetHealedEvent>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var (presenterHolderRef, currentHpRef, frameHpChangeRef, hpDataHolder) in SystemAPI
                .Query<
                    RefRO<PresenterHolder>
                    , RefRO<CurrentHp>
                    , RefRO<FrameHpChange>
                    , HpDataHolder>()
                .WithAll<
                    GetHealedEvent>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;

                int healValue = frameHpChangeRef.ValueRO;
                float remainingHpRatio = (float)currentHpRef.ValueRO / hpDataHolder.valueOfHpData.MaxHp;

                basePresenter.Messenger.MessagePublisher
                    .Publish(new OnHealedMessage(healValue, remainingHpRatio));
            }

        }

    }

}
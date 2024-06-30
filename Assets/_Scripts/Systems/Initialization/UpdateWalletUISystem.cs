using Unity.Entities;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;
using Components.GameResource;
using Unity.Scenes;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(SceneSystemGroup))]
    public partial class UpdateWalletUISystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    WalletChanged
                    , ResourceWalletElement>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnStartRunning()
        {
            // Update first time.
            foreach (var resourceWallet in
                SystemAPI.Query<DynamicBuffer<ResourceWalletElement>>())
            {
                this.UpdateUI(resourceWallet);
            }
        }

        protected override void OnUpdate()
        {

            foreach (var (walletChangedRef, resourceWallet) in
                SystemAPI.Query<
                    RefRW<WalletChanged>
                    , DynamicBuffer<ResourceWalletElement>>())
            {

                if (!walletChangedRef.ValueRO.Value) continue;
                walletChangedRef.ValueRW.Value = false;

                this.UpdateUI(resourceWallet);

            }

        }
        
        private void UpdateUI(DynamicBuffer<ResourceWalletElement> resourceWallet)
        {
            foreach (var walletElement in resourceWallet)
            {
                GameplayMessenger.MessagePublisher.Publish(new ResourceDisplayMessage
                {
                    ResourceType = walletElement.Type,
                    Quantity = walletElement.Quantity,
                });
            }
        }


    }
}
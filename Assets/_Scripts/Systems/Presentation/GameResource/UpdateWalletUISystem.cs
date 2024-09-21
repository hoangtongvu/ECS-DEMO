using Unity.Entities;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;
using Components.GameResource;
using Components.Player;

namespace Systems.Presentation.GameResource
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class UpdateWalletUISystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceWalletElement
                    , WalletChangedTag
                    , PlayerTag>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnStartRunning()
        {
            // Update first time.
            foreach (var resourceWallet in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>>()
                    .WithAll<PlayerTag>())
            {
                this.UpdateUI(resourceWallet);
            }
        }

        protected override void OnUpdate()
        {

            foreach (var resourceWallet in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>>()
                    .WithAll<WalletChangedTag>()
                    .WithAll<PlayerTag>())
            {
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
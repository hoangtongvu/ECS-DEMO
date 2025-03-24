using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.EntitySpawningPanel.SpawningProfileDisplay
{
    public class IncSpawnCountButton : BaseButton
    {
        [SerializeField] private SpawningProfileDisplayCtrl displayCtrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.displayCtrl);
        }

        protected override void OnClick()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new SpawnUnitMessage(this.displayCtrl.SpawnerEntity, this.displayCtrl.SpawningProfileElementIndex));
            
        }

    }

}
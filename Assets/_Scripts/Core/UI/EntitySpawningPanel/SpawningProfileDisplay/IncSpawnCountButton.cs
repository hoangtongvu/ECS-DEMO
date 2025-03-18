using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.EntitySpawningPanel.SpawningProfileDisplay
{
    public class IncSpawnCountButton : BaseButton
    {
        [SerializeField] private SpawningProfileDisplayCtrl spawningProfileDisplayCtrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.spawningProfileDisplayCtrl);
        }

        protected override void OnClick()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new SpawnUnitMessage(this.spawningProfileDisplayCtrl.RuntimeUIID));
            
        }

    }
}
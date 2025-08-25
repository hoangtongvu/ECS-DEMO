using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel
{
    [GenerateUIType("BuildableObjectsPanel")]
    public partial class BuildableObjectsPanelCtrl : BaseUICtrl
    {
        private ISubscription subscription;
        [SerializeField] private ObjectDisplaysHolder objectDisplaysHolder;

        public ObjectDisplaysHolder ObjectDisplaysHolder => objectDisplaysHolder;

        private void Start()
        {
            this.gameObject.SetActive(false);
            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<BuildModeToggleMessage>(this.HandleMessage);

        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.objectDisplaysHolder);
        }

        private void OnDestroy() => this.subscription.Dispose();

        private void HandleMessage(BuildModeToggleMessage message)
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}
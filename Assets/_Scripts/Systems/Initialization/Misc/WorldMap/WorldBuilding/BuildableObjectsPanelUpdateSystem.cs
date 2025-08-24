using Components.Misc.WorldMap.WorldBuilding;
using Components.UI.Pooling;
using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.Identification;
using Core.UI.Pooling;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BuildableObjectsPanelUpdateSystem : SystemBase
    {
        private NativeQueue<BuildModeToggleMessage> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);

            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<BuildModeToggleMessage>(message => this.messageQueue.Enqueue(message));

            this.RequireForUpdate<PlayerBuildableObjectElement>();
            this.RequireForUpdate<UIPoolMapInitializedTag>();
            this.RequireForUpdate<BuildableObjectsPanelHolder>();
        }

        protected override void OnUpdate()
        {
            var buildableObjectsPanelHolder = SystemAPI.GetSingleton<BuildableObjectsPanelHolder>();
            var buildableObjects = SystemAPI.GetSingletonBuffer<PlayerBuildableObjectElement>();

            while (this.messageQueue.TryDequeue(out var data))
            {
                var buildableObjectsPanel = buildableObjectsPanelHolder.Value.Value;
                var displaysHolder = buildableObjectsPanel.ObjectDisplaysHolder;

                // Despawn all existing DisplayPanel
                foreach (var displayCtrl in displaysHolder.Displays)
                {
                    UICtrlPoolMap.Instance.Return(displayCtrl);
                }

                displaysHolder.Displays.Clear();

                int index = 0;
                // Respawn all
                foreach (var playerBuildableObjectElement in buildableObjects)
                {
                    var buildableObjectDisplayCtrl = (BuildableObjectDisplayCtrl)UICtrlPoolMap.Instance.Rent(UIType.BuildableObjectDisplay);

                    buildableObjectDisplayCtrl.gameObject.SetActive(true);
                    buildableObjectDisplayCtrl.transform.SetParent(displaysHolder.transform);

                    buildableObjectDisplayCtrl.IndexInDisplaysHolder = index;
                    buildableObjectDisplayCtrl.DisplayPreviewImage.Image.sprite = playerBuildableObjectElement.PreviewSprite;
                    buildableObjectDisplayCtrl.BuildNameText.SetName(playerBuildableObjectElement.Name.ToString());

                    displaysHolder.Displays.Add(buildableObjectDisplayCtrl);
                    index++;
                }

            }

        }

        protected override void OnDestroy() => this.subscription.Dispose();

    }

}
using Components.ComponentMap;
using Components.Misc.WorldMap.WorldBuilding;
using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using Core.UI;
using Core.UI.Identification;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay;
using Core.Utilities.Helpers;
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

            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<BuildableObjectsPanelRuntimeUIID>();
            this.RequireForUpdate<PlayerBuildableObjectElement>();
        }

        protected override void OnUpdate()
        {
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiRuntimeID = SystemAPI.GetSingleton<BuildableObjectsPanelRuntimeUIID>().Value;
            var buildableObjects = SystemAPI.GetSingletonBuffer<PlayerBuildableObjectElement>();

            while (this.messageQueue.TryDequeue(out var data))
            {
                bool canGetUI = spawnedUIMap.Value.TryGetValue(uiRuntimeID, out BaseUICtrl baseUICtrl);

                if (!canGetUI)
                    throw new System.Exception($"Can't get UI of type {nameof(BuildableObjectsPanelCtrl)} with RuntimeUIID = {uiRuntimeID}");

                var buildableObjectsPanel = (BuildableObjectsPanelCtrl)baseUICtrl;
                var displaysHolder = buildableObjectsPanel.ObjectDisplaysHolder;

                // Despawn all existing DisplayPanel
                foreach (var displayCtrl in displaysHolder.Displays)
                {
                    displayCtrl.Despawn(uiPrefabAndPoolMap.Value, spawnedUIMap.Value);
                }

                displaysHolder.Displays.Clear();

                int index = 0;
                // Respawn all
                foreach (var playerBuildableObjectElement in buildableObjects)
                {
                    var buildableObjectDisplayCtrl = (BuildableObjectDisplayCtrl)UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap.Value
                        , spawnedUIMap.Value
                        , UIType.BuildableObjectDisplay);

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
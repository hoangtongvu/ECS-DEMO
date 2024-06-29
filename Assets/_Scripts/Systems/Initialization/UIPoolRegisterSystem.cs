using Components.ComponentMap;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.MyCanvas;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class UIPoolRegisterSystem : SystemBase
    {
        private NativeQueue<UIPoolRegisterMessage> eventQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.eventQueue = new NativeQueue<UIPoolRegisterMessage>(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<UIPoolRegisterMessage>(this.HandleEvent);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            UIPoolMap uiMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();

            while (this.eventQueue.TryDequeue(out var data))
            {
                if (this.CanvasTypeIsNone(data))
                {
                    UnityEngine.Debug.LogError($"UI with type: {data.Type} has CanvasType = {CanvasType.None}");
                    continue;
                }


                if (!uiMap.Value
                    .TryAdd(
                        data.Type
                        , new UIPoolMapValue
                        {
                            GlobalID = 0,
                            Prefab = data.Prefab,
                            UIPool = data.UIPool,
                            DefaultHolderTransform = data.UIPool.Value.transform,
                        }))
                {
                    UnityEngine.Debug.LogError($"Another BaseUICtrl has already been registered with UIType = {data.Type}");
                }
            }
        }

        protected override void OnDestroy()
        {
            this.subscription.Unsubscribe();
            this.eventQueue.Dispose();
        }


        private void HandleEvent(UIPoolRegisterMessage data) => this.eventQueue.Enqueue(data);

        private bool CanvasTypeIsNone(UIPoolRegisterMessage data) => data.CanvasType == CanvasType.None;


    }
}
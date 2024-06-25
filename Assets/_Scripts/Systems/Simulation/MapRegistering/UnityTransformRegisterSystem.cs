using Components.ComponentMap;
using Components.CustomIdentification;
using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.MapRegistering
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    public partial class UnityTransformRegisterSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<UniqueId, UnityEngine.Transform>> eventQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.CreateTransformMap();

            this.eventQueue = new NativeQueue<RegisterMessage<UniqueId, UnityEngine.Transform>>(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<UniqueId, UnityEngine.Transform>>(this.HandleMessage);
        }

        protected override void OnUpdate()
        {
            while (this.eventQueue.TryDequeue(out var data))
            {
                this.HandleEvent(in data);
            }
        }

        protected override void OnDestroy()
        {
            this.subscription.Unsubscribe();
            this.eventQueue.Dispose();
        }


        private void HandleMessage(RegisterMessage<UniqueId, UnityEngine.Transform> data) => this.eventQueue.Enqueue(data);

        private void HandleEvent(in RegisterMessage<UniqueId, UnityEngine.Transform> data)
        {
            var transformMap = SystemAPI.ManagedAPI.GetSingleton<UnityTransformMap>();

            if (transformMap.Value.TryAdd(
                new UniqueIdICD
                {
                    BaseId = data.ID,
                }
                , data.TargetRef.Value)) return;

            UnityEngine.Debug.LogError($"Một Unity Transform khác đã được đăng ký với Id={data.ID}", data.TargetRef.Value);

        }

        private void CreateTransformMap()
        {
            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new UnityTransformMap
                {
                    Value = new System.Collections.Generic.Dictionary<UniqueIdICD, UnityEngine.Transform>()
                });
        }


    }
}
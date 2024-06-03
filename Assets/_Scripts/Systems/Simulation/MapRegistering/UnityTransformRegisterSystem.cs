using Components.ComponentMap;
using Components.CustomIdentification;
using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    public partial class UnityTransformRegisterSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<UniqueId, UnityEngine.Transform>> eventDataQueue;


        protected override void OnCreate()
        {
            this.eventDataQueue = new(Allocator.Persistent);

            MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<UniqueId, UnityEngine.Transform>>(this.HandleEvent0);
        }

        protected override void OnUpdate()
        {
            while (this.eventDataQueue.TryDequeue(out var message))
            {
                var transformMap = SystemAPI.ManagedAPI.GetSingleton<UnityTransformMap>();

                if (transformMap.Value.TryAdd(
                    new UniqueIdICD
                    {
                        BaseId = message.ID,
                    }
                    , message.TargetRef.Value)) continue;

                UnityEngine.Debug.LogError($"Một Unity Transform khác đã được đăng ký với Id={message.ID}", message.TargetRef.Value);
            }
        }

        private void HandleEvent(RegisterMessage<UniqueId, UnityEngine.Transform> data) => this.eventDataQueue.Enqueue(data);

        private void HandleEvent0(RegisterMessage<UniqueId, UnityEngine.Transform> data)
        {
            var transformMap = SystemAPI.ManagedAPI.GetSingleton<UnityTransformMap>();

            if (transformMap.Value.TryAdd(
                new UniqueIdICD
                {
                    BaseId = data.ID,
                }
                , data.TargetRef.Value)) return;
            UnityEngine.Debug.LogError($"Một Unity Transform khác đã được đăng ký với Id={data.ID}", data.TargetRef.Value);


            // 2 Jobs:
            // - Add into dict.
            // - Add into event buffer.
        }



    }
}
using Components.ComponentMap;
using Components.CustomIdentification;
using Core.Animator;
using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.MapRegistering
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    public partial class BaseAnimatorRegisterSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<UniqueId, BaseAnimator>> eventQueue;
        private ISubscription subscription;


        protected override void OnCreate()
        {
            this.CreateBaseAnimMap();

            this.eventQueue = new NativeQueue<RegisterMessage<UniqueId, BaseAnimator>>(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<UniqueId, BaseAnimator>>(this.HandleMessage);
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


        private void HandleEvent(in RegisterMessage<UniqueId, BaseAnimator> data)
        {
            var animatorMap = SystemAPI.ManagedAPI.GetSingleton<BaseAnimatorMap>();

            var uniqueIdICD = new UniqueIdICD
            {
                BaseId = data.ID,
            };

            BaseAnimator target = data.TargetRef.Value;

            if (animatorMap.Value.TryAdd(uniqueIdICD, target)) return;

            UnityEngine.Debug.LogError($"Một BaseAnimator khác đã được đăng ký với Id={data.ID}", target);
        }

        private void HandleMessage(RegisterMessage<UniqueId, BaseAnimator> data) => this.eventQueue.Enqueue(data);

        private void CreateBaseAnimMap()
        {
            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new BaseAnimatorMap
                {
                    Value = new Dictionary<UniqueIdICD, BaseAnimator>()
                });
        }

    }
}
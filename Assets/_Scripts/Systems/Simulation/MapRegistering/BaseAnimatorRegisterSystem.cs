using Components.ComponentMap;
using Components.CustomIdentification;
using Core.Animator;
using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using System.Collections.Generic;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class BaseAnimatorRegisterSystem : SystemBase
    {
        private ISubscription subscription;


        protected override void OnCreate()
        {
            this.CreateBaseAnimMap();

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<UniqueId, BaseAnimator>>(this.HandleEvent);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        protected override void OnDestroy()
        {
            this.subscription.Unsubscribe();
        }


        private void HandleEvent(RegisterMessage<UniqueId, BaseAnimator> data)
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
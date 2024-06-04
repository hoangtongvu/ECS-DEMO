using Components.ComponentMap;
using Components.CustomIdentification;
using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class UnityTransformRegisterSystem : SystemBase
    {
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.CreateTransformMap();

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<UniqueId, UnityEngine.Transform>>(this.HandleEvent);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        protected override void OnDestroy()
        {
            this.subscription.Unsubscribe();
        }


        private void HandleEvent(RegisterMessage<UniqueId, UnityEngine.Transform> data)
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
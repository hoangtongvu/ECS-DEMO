using Components.Unit.Misc;
using Core.MyEvent.PubSub.Messengers;
using Core.Unit.Misc;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class RecruitUnitMessageListUpdateSystem : SystemBase
    {
        private NativeList<RecruitUnitMessage> messages;

        protected override void OnCreate()
        {
            const int listLength = 5;

            this.messages = new NativeList<RecruitUnitMessage>(listLength, Allocator.Persistent);

            GameplayMessenger.MessageSubscriber
                .Subscribe<RecruitUnitMessage>(message => this.messages.Add(message));

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new RecruitUnitMessageList
                {
                    Value = new NativeList<RecruitUnitMessage>(listLength, Allocator.Persistent),
                });

        }

        protected override void OnUpdate()
        {
            var recruitUnitMessageList = SystemAPI.GetSingleton<RecruitUnitMessageList>();

            recruitUnitMessageList.Value.CopyFrom(in this.messages);
            this.messages.Clear();

        }

    }

}
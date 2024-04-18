using Core.MyEvent.PubSub.Messengers;
using Core.UI.Identification;
using Unity.Entities;
using Unity.Mathematics;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class TestSpawnUISystem : SystemBase
    {

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            // Spawn without Optional Parent Transform.

            for (int i = 0; i < 2; i++)
            {
                GameplayMessenger.MessagePublisher.Publish(new UISpawnMessage
                {
                    UIType = UIType.HouseUI,
                    Position = new(0, 5 * i, 0),
                    Quaternion = quaternion.identity,
                });

                
            }

            // Spawn with Optional Parent Transform.
            for (uint i = 1; i < 4; i++)
            {
                GameplayMessenger.MessagePublisher.Publish(new UISpawnMessage
                {
                    UIType = UIType.HouseUI,
                    Position = new(0, 5 * i, 0),
                    Quaternion = quaternion.identity,
                    ParentUIID = new UIID
                    {
                        LocalId = i,
                        Type = UIType.HouseUI,
                    }
                });

                
            }



        }

    }
}
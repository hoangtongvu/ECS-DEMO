using Components.Misc.TutorialMessage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.TutorialMessage.MessageAdders
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(MovementControllersMessageAdderSystem))]
    [BurstCompile]
    public partial struct AttackControllersMessageAdderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TutorialMessageList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var tutorialMessageList = SystemAPI.GetSingleton<TutorialMessageList>().Value;

            tutorialMessageList.Add(new()
            {
                String = "Press <b>[Left Mouse Button]</b> to attack",
                TextDuration = new(10f),
            });
        }

    }

}
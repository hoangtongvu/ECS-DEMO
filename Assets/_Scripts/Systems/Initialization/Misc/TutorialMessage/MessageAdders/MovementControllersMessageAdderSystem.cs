using Components.Misc.TutorialMessage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.TutorialMessage.MessageAdders
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct MovementControllersMessageAdderSystem : ISystem
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
                String = "Press <b>[A][W][S][D]</b> to move around",
                TextDuration = new(10f),
            });

            tutorialMessageList.Add(new()
            {
                String = "Hold <b>[Shift]</b> to run",
                TextDuration = new(10f),
            });
        }

    }

}
using Components.Misc.TutorialMessage;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.TutorialMessage.MessageAdders
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct RecruitUnitMessageAdderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitTag>();
            state.RequireForUpdate<TutorialMessageList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var tutorialMessageList = SystemAPI.GetSingleton<TutorialMessageList>().Value;

            tutorialMessageList.Add(new()
            {
                String = "Find the nearest <b>Tent</b>, stand near an <b>Unit</b>, and Press <b>[E]</b> to <b>recruit</b> him",
                TextDuration = new(10f),
            });
        }

    }

}
using Components.Misc.TutorialMessage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.TutorialMessage
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct TutorialMessageDequeueSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    TutorialMessageList
                    , TutorialMessageTimerSeconds
                    , TutorialMessageCanDespawnState>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tutorialMessageList = SystemAPI.GetSingleton<TutorialMessageList>().Value;
            int length = tutorialMessageList.Length;

            if (length == 0) return;

            var firstMessageElement = tutorialMessageList[0];
            var timerSecondsRef = SystemAPI.GetSingletonRW<TutorialMessageTimerSeconds>();

            timerSecondsRef.ValueRW.Value += SystemAPI.Time.DeltaTime;

            if (timerSecondsRef.ValueRO.Value < firstMessageElement.TextDuration) return;

            tutorialMessageList.RemoveAt(0);
            timerSecondsRef.ValueRW.Value = 0;
            SystemAPI.GetSingletonRW<TutorialMessageCanDespawnState>().ValueRW.Value = true;
        }

    }

}
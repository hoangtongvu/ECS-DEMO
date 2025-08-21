using Components.Misc.TutorialMessage;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.TutorialMessage
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct TutorialMessageComponentsInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new TutorialMessageSpawnedState());
            su.AddOrSetComponentData(new TutorialMessageCanDespawnState());
            su.AddOrSetComponentData(new SpawnedTutorialMessageCtrlHolder());
            su.AddOrSetComponentData(new TutorialMessageTimerSeconds());
            su.AddOrSetComponentData(new TutorialMessageList
            {
                Value = new(Allocator.Persistent),
            });

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

    }

}
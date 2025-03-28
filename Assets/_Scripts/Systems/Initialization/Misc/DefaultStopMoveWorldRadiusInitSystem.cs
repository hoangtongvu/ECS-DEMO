using Components.Misc;
using Unity.Burst;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct DefaultStopMoveWorldRadiusInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new DefaultStopMoveWorldRadius
                {
                    Value = new(0.1f),
                });

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
        }

    }

}
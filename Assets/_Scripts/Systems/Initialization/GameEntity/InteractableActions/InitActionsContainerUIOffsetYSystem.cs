using Components.GameEntity.InteractableActions;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Utilities;

namespace Systems.Initialization.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct InitActionsContainerUIOffsetYSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ActionsContainerUIOffsetY
                {
                    Value = (half)4f,
                });

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

    }

}
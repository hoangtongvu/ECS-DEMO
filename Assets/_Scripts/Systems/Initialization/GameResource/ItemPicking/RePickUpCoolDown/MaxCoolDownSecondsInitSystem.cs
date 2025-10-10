using Unity.Entities;
using Unity.Burst;
using Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;
using Utilities;

namespace Systems.Initialization.GameResource.ItemPicking.RePickUpCoolDown
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct MaxCoolDownSecondsInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new PreviousPickerPickupMaxCoolDownSeconds(4f));
        }

    }

}
using Components.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;
using Unity.Burst;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct ExitBuildModeButton_CDInitSystem : ISystem
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

            su.AddComponent<ExitBuildModeButton_CD.Holder>();
            su.AddComponent<ExitBuildModeButton_CD.CanShow>();
            su.SetComponentEnabled<ExitBuildModeButton_CD.CanShow>(false);
            su.AddComponent<ExitBuildModeButton_CD.IsActive>();
            su.SetComponentEnabled<ExitBuildModeButton_CD.IsActive>(false);
        }

    }

}

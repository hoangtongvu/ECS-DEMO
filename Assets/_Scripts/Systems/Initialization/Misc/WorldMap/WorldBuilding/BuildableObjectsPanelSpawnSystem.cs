using Unity.Entities;
using Unity.Burst;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Components.Misc.WorldMap.WorldBuilding;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct BuildableObjectsPanelSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var runtimeUIIDRef = SystemAPI.GetSingletonRW<BuildableObjectsPanelRuntimeUIID>();

            runtimeUIIDRef.ValueRW.Value =
                UISpawningHelper.Spawn(uiPrefabAndPoolMap, spawnedUIMap, UIType.BuildableObjectsPanel).RuntimeUIID;

        }

    }

}
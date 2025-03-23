using Unity.Entities;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Components.Misc.WorldMap.WorldBuilding;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanelSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<BuildableObjectsPanelRuntimeUIID>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var runtimeUIIDRef = SystemAPI.GetSingletonRW<BuildableObjectsPanelRuntimeUIID>();

            runtimeUIIDRef.ValueRW.Value =
                UISpawningHelper.Spawn(uiPrefabAndPoolMap, spawnedUIMap, UIType.BuildableObjectsPanel).RuntimeUIID;

        }

    }

}
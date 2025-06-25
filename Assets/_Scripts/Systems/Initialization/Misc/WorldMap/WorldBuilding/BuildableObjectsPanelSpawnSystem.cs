using Unity.Entities;
using Components.ComponentMap;
using Core.UI.Identification;
using Core.Utilities.Helpers;
using Components.Misc.WorldMap.WorldBuilding;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanelSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<BuildableObjectsPanelRuntimeUIID>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var runtimeUIIDRef = SystemAPI.GetSingletonRW<BuildableObjectsPanelRuntimeUIID>();

            runtimeUIIDRef.ValueRW.Value =
                UISpawningHelper.Spawn(uiPrefabAndPoolMap.Value, spawnedUIMap.Value, UIType.BuildableObjectsPanel).RuntimeUIID;

        }

    }

}
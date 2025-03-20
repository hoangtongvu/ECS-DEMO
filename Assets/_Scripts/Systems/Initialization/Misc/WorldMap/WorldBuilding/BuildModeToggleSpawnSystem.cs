using Unity.Entities;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Utilities;
using Components.Misc.WorldMap.WorldBuilding;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BuildModeToggleSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<BuildableObjectsPanelRuntimeUIID>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();

            UISpawningHelper.Spawn(uiPrefabAndPoolMap, spawnedUIMap, UIType.BuildModeTrigger);

        }

    }

}
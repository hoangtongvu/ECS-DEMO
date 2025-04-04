using Unity.Entities;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class BuildModeToggleSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();
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
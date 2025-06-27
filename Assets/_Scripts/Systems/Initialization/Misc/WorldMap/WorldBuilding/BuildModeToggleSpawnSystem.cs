using Unity.Entities;
using Components.ComponentMap;
using Core.UI.Identification;
using Core.Utilities.Helpers;

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

            UISpawningHelper.Spawn(uiPrefabAndPoolMap.Value, spawnedUIMap.Value, UIType.BuildModeTrigger);

        }

    }

}
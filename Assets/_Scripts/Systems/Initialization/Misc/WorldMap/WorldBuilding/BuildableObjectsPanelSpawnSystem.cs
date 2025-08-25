using Unity.Entities;
using Core.UI.Identification;
using Components.Misc.WorldMap.WorldBuilding;
using Utilities;
using Core.UI.Pooling;
using Components.UI.Pooling;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanelSpawnSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<UIPoolMapInitializedTag>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<BuildableObjectsPanelHolder>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var buildableObjectsPanelHolderRef = SystemAPI.GetSingletonRW<BuildableObjectsPanelHolder>();

            buildableObjectsPanelHolderRef.ValueRW.Value =
                (BuildableObjectsPanelCtrl)UICtrlPoolMap.Instance.Rent(UIType.BuildableObjectsPanel);

        }

    }

}
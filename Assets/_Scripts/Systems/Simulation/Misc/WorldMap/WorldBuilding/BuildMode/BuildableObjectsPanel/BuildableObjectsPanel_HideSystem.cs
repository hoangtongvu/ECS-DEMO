using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Core.UI;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanel_HideSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    BuildableObjectsPanel_CD.Holder
                    , BuildableObjectsPanel_CD.CanShow
                    , BuildableObjectsPanel_CD.IsActive>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var uiHolderRef in SystemAPI
                .Query<
                    RefRW<BuildableObjectsPanel_CD.Holder>>()
                .WithDisabled<
                    BuildableObjectsPanel_CD.CanShow>()
                .WithDisabled<
                    BuildableObjectsPanel_CD.IsActive>())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;
                if (uiCtrl == null) continue;

                if (uiCtrl.State != UIState.Hidden) continue;
                uiHolderRef.ValueRW.Value = null;
            }

        }

    }

}
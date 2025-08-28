using Unity.Entities;
using Core.UI.Identification;
using Core.UI.Pooling;
using Components.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;
using Core.UI.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ExitBuildModeButton_ShowSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ExitBuildModeButton_CD.Holder
                    , ExitBuildModeButton_CD.CanShow
                    , ExitBuildModeButton_CD.IsVisible>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (uiHolderRef, entity) in SystemAPI
                .Query<
                    RefRW<ExitBuildModeButton_CD.Holder>>()
                .WithAll<
                    ExitBuildModeButton_CD.CanShow>()
                .WithDisabled<
                    ExitBuildModeButton_CD.IsVisible>()
                .WithEntityAccess())
            {
                var objectsPanelCtrl = (ExitBuildModeButtonCtrl)UICtrlPoolMap.Instance
                    .Rent(UIType.ExitBuildModeButton);

                uiHolderRef.ValueRW.Value = objectsPanelCtrl;
                objectsPanelCtrl.gameObject.SetActive(true);

                SystemAPI.SetComponentEnabled<ExitBuildModeButton_CD.IsVisible>(entity, true);

                if (!SystemAPI.HasComponent<ExitBuildModeButton_CD.CanUpdate>(entity)) continue;
                SystemAPI.SetComponentEnabled<ExitBuildModeButton_CD.CanUpdate>(entity, true);
            }

        }

    }

}
using Components.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;
using Core.UI;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ExitBuildModeButton_HideSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ExitBuildModeButton_CD.Holder
                    , ExitBuildModeButton_CD.CanShow
                    , ExitBuildModeButton_CD.IsActive>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var uiHolderRef in SystemAPI
                .Query<
                    RefRW<ExitBuildModeButton_CD.Holder>>()
                .WithDisabled<
                    ExitBuildModeButton_CD.CanShow>()
                .WithDisabled<
                    ExitBuildModeButton_CD.IsActive>())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;
                if (uiCtrl == null) continue;

                if (uiCtrl.State != UIState.Hidden) continue;
                uiHolderRef.ValueRW.Value = null;
            }

        }

    }

}
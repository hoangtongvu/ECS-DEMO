using Components.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;
using Core.UI.Identification;
using Core.UI.Pooling;
using Core.UI.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;
using Unity.Entities;

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
                    , ExitBuildModeButton_CD.IsActive>()
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
                    ExitBuildModeButton_CD.IsActive>()
                .WithEntityAccess())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;
                bool isUIHidden = uiCtrl == null;

                if (isUIHidden)
                {
                    uiCtrl = (ExitBuildModeButtonCtrl)UICtrlPoolMap.Instance
                        .Rent(UIType.ExitBuildModeButton);

                    uiHolderRef.ValueRW.Value = uiCtrl;
                    uiCtrl.gameObject.SetActive(true);
                }
                else
                {
                    uiCtrl.Reuse();
                }

                SystemAPI.SetComponentEnabled<ExitBuildModeButton_CD.IsActive>(entity, true);

                if (!SystemAPI.HasComponent<ExitBuildModeButton_CD.CanUpdate>(entity)) continue;
                SystemAPI.SetComponentEnabled<ExitBuildModeButton_CD.CanUpdate>(entity, true);
            }

        }

    }

}
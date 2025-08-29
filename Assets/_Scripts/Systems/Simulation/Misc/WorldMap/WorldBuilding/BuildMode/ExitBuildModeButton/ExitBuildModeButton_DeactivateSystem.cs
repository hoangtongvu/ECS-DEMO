using Components.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ExitBuildModeButton_DeactivateSystem : SystemBase
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
                .WithDisabled<
                    ExitBuildModeButton_CD.CanShow>()
                .WithAll<
                    ExitBuildModeButton_CD.IsActive>()
                .WithEntityAccess())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;

                uiCtrl.TriggerHiding();
                SystemAPI.SetComponentEnabled<ExitBuildModeButton_CD.IsActive>(entity, false);
            }

        }

    }

}
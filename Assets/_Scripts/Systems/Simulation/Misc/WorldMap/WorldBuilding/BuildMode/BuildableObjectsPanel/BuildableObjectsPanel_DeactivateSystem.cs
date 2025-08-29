using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanel_DeactivateSystem : SystemBase
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
            foreach (var (uiHolderRef, choiceIndexRef, entity) in SystemAPI
                .Query<
                    RefRO<BuildableObjectsPanel_CD.Holder>
                    , RefRW<BuildableObjectChoiceIndex>>()
                .WithDisabled<
                    BuildableObjectsPanel_CD.CanShow>()
                .WithAll<
                    BuildableObjectsPanel_CD.IsActive>()
                .WithEntityAccess())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;

                uiCtrl.TriggerHiding();
                choiceIndexRef.ValueRW.Value = BuildableObjectChoiceIndex.NoChoice;
                SystemAPI.SetComponentEnabled<BuildableObjectsPanel_CD.IsActive>(entity, false);
            }

        }

    }

}
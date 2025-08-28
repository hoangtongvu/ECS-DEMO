using Unity.Entities;
using Core.UI.Identification;
using Components.Misc.WorldMap.WorldBuilding;
using Core.UI.Pooling;
using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanel_ShowSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    BuildableObjectsPanel_CD.Holder
                    , BuildableObjectsPanel_CD.CanShow
                    , BuildableObjectsPanel_CD.IsVisible>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (uiHolderRef, choiceIndexRef, entity) in SystemAPI
                .Query<
                    RefRW<BuildableObjectsPanel_CD.Holder>
                    , RefRW<BuildableObjectChoiceIndex>>()
                .WithAll<
                    BuildableObjectsPanel_CD.CanShow>()
                .WithDisabled<
                    BuildableObjectsPanel_CD.IsVisible>()
                .WithEntityAccess())
            {
                var objectsPanelCtrl = (BuildableObjectsPanelCtrl)UICtrlPoolMap.Instance
                    .Rent(UIType.BuildableObjectsPanel);

                uiHolderRef.ValueRW.Value = objectsPanelCtrl;
                objectsPanelCtrl.gameObject.SetActive(true);

                choiceIndexRef.ValueRW.Value = BuildableObjectChoiceIndex.NoChoice;

                SystemAPI.SetComponentEnabled<BuildableObjectsPanel_CD.IsVisible>(entity, true);

                if (!SystemAPI.HasComponent<BuildableObjectsPanel_CD.CanUpdate>(entity)) continue;
                SystemAPI.SetComponentEnabled<BuildableObjectsPanel_CD.CanUpdate>(entity, true);
            }

        }

    }

}
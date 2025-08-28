using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
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
                .WithDisabled<
                    BuildableObjectsPanel_CD.CanShow>()
                .WithAll<
                    BuildableObjectsPanel_CD.IsVisible>()
                .WithEntityAccess())
            {
                uiHolderRef.ValueRO.Value.Value.ReturnSelfToPool();
                uiHolderRef.ValueRW.Value = null;

                choiceIndexRef.ValueRW.Value = BuildableObjectChoiceIndex.NoChoice;

                SystemAPI.SetComponentEnabled<BuildableObjectsPanel_CD.IsVisible>(entity, false);
            }

        }

    }

}
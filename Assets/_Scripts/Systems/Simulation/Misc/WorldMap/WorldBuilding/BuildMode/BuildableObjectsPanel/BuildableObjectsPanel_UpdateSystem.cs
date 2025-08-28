using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Core.UI.Identification;
using Core.UI.Pooling;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanel_UpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerBuildableObjectElement>()
                .WithAll<
                    BuildableObjectsPanel_CD.Holder
                    , BuildableObjectsPanel_CD.CanUpdate
                    , BuildableObjectsPanel_CD.IsVisible>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (uiHolderRef, buildableObjects, entity) in SystemAPI
                .Query<
                    RefRO<BuildableObjectsPanel_CD.Holder>
                    , DynamicBuffer<PlayerBuildableObjectElement>>()
                .WithAll<
                    BuildableObjectsPanel_CD.CanUpdate>()
                .WithAll<
                    BuildableObjectsPanel_CD.IsVisible>()
                .WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<BuildableObjectsPanel_CD.CanUpdate>(entity, false);

                var buildableObjectsPanel = uiHolderRef.ValueRO.Value.Value;
                var displaysHolder = buildableObjectsPanel.ObjectDisplaysHolder;

                int index = 0;
                foreach (var buildableObject in buildableObjects)
                {
                    var objectDisplayCtrl = (BuildableObjectDisplayCtrl)UICtrlPoolMap.Instance
                        .Rent(UIType.BuildableObjectDisplay);

                    objectDisplayCtrl.gameObject.SetActive(true);
                    objectDisplayCtrl.transform.SetParent(displaysHolder.transform);

                    objectDisplayCtrl.IndexInDisplaysHolder = index;
                    objectDisplayCtrl.DisplayPreviewImage.Image.sprite = buildableObject.PreviewSprite;
                    objectDisplayCtrl.BuildNameText.SetName(buildableObject.Name.ToString());

                    displaysHolder.Displays.Add(objectDisplayCtrl);
                    index++;
                }

            }

        }

    }

}
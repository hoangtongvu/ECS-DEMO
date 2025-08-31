using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Core.UI.Identification;
using Core.UI.Pooling;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay;
using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews.CostStack;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class BuildableObjectsPanel_UpdateSystem : SystemBase
    {
        private Unity.Mathematics.Random rand;

        protected override void OnCreate()
        {
            this.rand = new(47);

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerBuildableObjectElement>()
                .WithAll<
                    BuildableObjectsPanel_CD.Holder
                    , BuildableObjectsPanel_CD.CanUpdate
                    , BuildableObjectsPanel_CD.IsActive>()
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
                    BuildableObjectsPanel_CD.IsActive>()
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

                    displaysHolder.Displays.Add(objectDisplayCtrl);

                    var costStacksHolder = objectDisplayCtrl.PreviewsCtrl.CostStacksHolder;
                    int costCount = this.rand.NextInt(1, 4);

                    for (int i = 0; i < costCount; i++)
                    {
                        float3 randomColor = this.rand.NextFloat3();
                        var costStack = (CostStackCtrl)UICtrlPoolMap.Instance
                            .Rent(UIType.CostStack);

                        costStack.transform.SetParent(costStacksHolder.transform, false);
                        costStack.ContainerLength = costCount;
                        costStack.IndexInContainer = i;
                        costStack.Image.color = new()
                        {
                            r = randomColor.x,
                            g = randomColor.y,
                            b = randomColor.z,
                            a = 255f,
                        };

                        costStack.gameObject.SetActive(true);
                        costStacksHolder.Value.Add(costStack);
                    }

                    index++;
                }

            }

        }

    }

}
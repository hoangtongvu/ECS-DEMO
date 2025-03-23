using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.PlacementPreview
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlacementPreviewDataUpdateSystem))]
    public partial class PlacementPreviewShowSystem : SystemBase
    {
        private EntityQuery placementPreviewSpriteQuery;

        protected override void OnCreate()
        {
            this.placementPreviewSpriteQuery = SystemAPI.QueryBuilder()
                .WithAllRW<LocalTransform, SpriteRenderer>()
                .WithAll<PlacementPreviewSpriteTag>()
                .Build();

            this.RequireForUpdate(this.placementPreviewSpriteQuery);
            this.RequireForUpdate<PlacementPreviewData>();
        }

        protected override void OnUpdate()
        {
            this.EntityManager.CompleteDependencyBeforeRW<LocalTransform>();
            var placementPreviewTransformRef = this.placementPreviewSpriteQuery.GetSingletonRW<LocalTransform>();
            int choiceIndex = SystemAPI.GetSingleton<BuildableObjectChoiceIndex>().Value;

            if (choiceIndex == BuildableObjectChoiceIndex.NoChoice)
            {
                this.HideSprite(ref placementPreviewTransformRef.ValueRW);
                return;
            }

            var placementData = SystemAPI.GetSingleton<PlacementPreviewData>();

            if (!placementData.CanPlacementPreview)
            {
                this.HideSprite(ref placementPreviewTransformRef.ValueRW);
                return;
            }

            placementPreviewTransformRef.ValueRW.Position = placementData.BuildingCenterPosOnGround.Add(y: 0.2f);
            placementPreviewTransformRef.ValueRW.Scale = placementData.PlacementSpriteScale;

            var spriteRenderer = this.placementPreviewSpriteQuery.GetSingletonRW<SpriteRenderer>();
            float spriteAlpha = spriteRenderer.color.a;

            Color newColor = placementData.IsBuildable ? Color.green : Color.red;
            newColor.a = spriteAlpha;

            spriteRenderer.color = newColor;

        }

        private void HideSprite(ref LocalTransform spriteTransform) => spriteTransform.Scale = 0;

    }

}

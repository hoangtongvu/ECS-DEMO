using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Core.Utilities.Extensions;
using Systems.Simulation.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Unity.Entities;
using UnityEngine;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlacementPreviewDataUpdateSystem))]
    public partial class PlayerBuildSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<PlacementPreviewData>();
        }

        protected override void OnUpdate()
        {
            int choiceIndex = SystemAPI.GetSingleton<BuildableObjectChoiceIndex>().Value;

            if (choiceIndex == BuildableObjectChoiceIndex.NoChoice) return;
            if (!Input.GetMouseButtonDown(0)) return;

            var placementPreviewData = SystemAPI.GetSingleton<PlacementPreviewData>();
            if (!placementPreviewData.CanPlacementPreview) return;
            if (!placementPreviewData.IsBuildable) return;

            var commandQueue = SystemAPI.GetSingleton<BuildCommandQueue>();
            var buildableObjectElement = SystemAPI.GetSingletonBuffer<PlayerBuildableObjectElement>()[choiceIndex];

            commandQueue.Value.Add(new()
            {
                Entity = buildableObjectElement.Entity,
                TopLeftCellGridPos = placementPreviewData.TopLeftCellGridPos,
                BuildingCenterPos = placementPreviewData.BuildingCenterPosOnGround.Add(y: buildableObjectElement.ObjectHeight),
                GridSquareSize = buildableObjectElement.GridSquareSize,
            });

        }

    }

}

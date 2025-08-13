using Components.Misc;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Components.MyCamera;
using Core.Misc;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Utilities;
using Utilities.Extensions;
using Utilities.Helpers;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.PlacementPreview
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class PlacementPreviewDataUpdateSystem : SystemBase
    {
        private UnityEngine.Camera mainCamera;

        protected override void OnCreate()
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<PlacementPreviewData>();

            this.RequireForUpdate<PlacementPreviewSpriteTag>();
            this.RequireForUpdate<BuildableObjectChoiceIndex>();
            this.RequireForUpdate<InputData>();
        }

        protected override void OnStartRunning()
        {
            this.mainCamera = SystemAPI.GetSingleton<MainCamHolder>().Value;
        }

        protected override void OnUpdate()
        {
            var inputData = SystemAPI.GetSingleton<InputData>();
            var placementPreviewDataRef = SystemAPI.GetSingletonRW<PlacementPreviewData>();

            bool mouseHoverOnUI = inputData.IsPointerOverGameObject;
            if (mouseHoverOnUI)
            {
                placementPreviewDataRef.ValueRW.CanPlacementPreview = false;
                return;
            }

            this.EntityManager.CompleteDependencyBeforeRW<LocalTransform>();
            int choiceIndex = SystemAPI.GetSingleton<BuildableObjectChoiceIndex>().Value;

            if (choiceIndex == BuildableObjectChoiceIndex.NoChoice) return;

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            bool canHit = this.CastRayToGround(in physicsWorld, out var raycastHit);

            if (!canHit)
            {
                placementPreviewDataRef.ValueRW.CanPlacementPreview = false;
                return;
            }

            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            var buildableObjectElement = SystemAPI.GetSingletonBuffer<PlayerBuildableObjectElement>()[choiceIndex];
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();

            WorldMapHelper.WorldPosToGridPos(in cellRadius, raycastHit.Position, out int2 gridPos);
            WorldMapHelper.GridPosToWorldPos(in cellRadius, in gridPos, out float3 centerPosOfCell);

            var gridSquareSize = buildableObjectElement.GameEntitySize.GridSquareSize;
            float3 spriteStartPos = centerPosOfCell;
            float plusAmount = (float)gridSquareSize * cellRadius - cellRadius;

            spriteStartPos = spriteStartPos.Add(x: plusAmount, z: -plusAmount);

            placementPreviewDataRef.ValueRW.CanPlacementPreview = true;
            placementPreviewDataRef.ValueRW.TopLeftCellGridPos = gridPos;
            placementPreviewDataRef.ValueRW.BuildingCenterPosOnGround = spriteStartPos.With(y: raycastHit.Position.y);
            placementPreviewDataRef.ValueRW.PlacementSpriteScale = cellRadius * 2 * 100 * gridSquareSize;
            placementPreviewDataRef.ValueRW.IsBuildable =
                this.AreAllCellsPassable(in costMap, in gridPos, gridSquareSize);

        }

        private bool CastRayToGround(in PhysicsWorldSingleton physicsWorld, out Unity.Physics.RaycastHit raycastHit)
        {
            UnityEngine.Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float3 rayStart = ray.origin;
            float3 rayEnd = ray.GetPoint(100f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = (uint)CollisionLayer.Ground,
                }
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

        private bool AreAllCellsPassable(
            in WorldTileCostMap costMap
            , in int2 topLeftGridPos
            , int squareSize)
        {
            for (int y = topLeftGridPos.y; y < topLeftGridPos.y + squareSize; y++)
            {
                for (int x = topLeftGridPos.x; x < topLeftGridPos.x + squareSize; x++)
                {
                    bool isValidGridPos = WorldMapHelper.IsValidGridPos(costMap.Width, costMap.Height, costMap.Offset, x, y);

                    if (!isValidGridPos) return false;

                    costMap.GetCellAt(x, y, out var cell);
                    if (cell.IsPassable()) continue;

                    return false;

                }
                
            }

            return true;
        }

    }

}

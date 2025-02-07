using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Utilities;
using UnityEngine;
using Components.Misc.WorldMap;
using Core.Misc.WorldMap;

namespace Systems.Presentation.Misc.WorldMap
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial class GridLinesDrawSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<WorldTileCostMap>();
            this.RequireForUpdate<MapDebugConfig>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<CellPresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var gridDebugConfig = SystemAPI.GetSingleton<MapDebugConfig>();
            if (!gridDebugConfig.ShowCellGridLines) return;

            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            float mapCellSize = SystemAPI.GetSingleton<MapCellSize>().Value;
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;

            int2 gridMapSize = new(costMap.Width, costMap.Height);

            GameObject gameObject = new("GridLineDrawer");
            GridLinesDrawer gridLinesDrawer = gameObject.AddComponent<GridLinesDrawer>();
            gridLinesDrawer.GridMapSize = gridMapSize;
            gridLinesDrawer.DrawCellSize = mapCellSize;
            gridLinesDrawer.GridOffset = gridOffset;
            gridLinesDrawer.DrawColor = Color.yellow;

        }


    }
}
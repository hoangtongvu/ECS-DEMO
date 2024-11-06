using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Components.Misc.FlowField;
using Utilities;
using Core.Misc.FlowField;
using UnityEngine;

namespace Systems.Presentation.Misc.FlowField
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial class GridLinesDrawSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<FlowFieldGridMap>();
            this.RequireForUpdate<GridDebugConfig>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<GridNodePresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var gridDebugConfig = SystemAPI.GetSingleton<GridDebugConfig>();
            if (!gridDebugConfig.ShowGridLines) return;
            
            int mapWidth = SystemAPI.GetSingleton<FlowFieldMapWidth>().Value;
            int mapHeight = SystemAPI.GetSingleton<FlowFieldMapHeight>().Value;
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;

            int2 gridMapSize = new(mapWidth, mapHeight);

            GameObject gameObject = new("GridLineDrawer");
            GridLinesDrawer gridLinesDrawer = gameObject.AddComponent<GridLinesDrawer>();
            gridLinesDrawer.GridMapSize = gridMapSize;
            gridLinesDrawer.DrawCellRadius = 0.5f;
            gridLinesDrawer.GridOffset = gridOffset;
            gridLinesDrawer.DrawColor = Color.yellow;

        }


    }
}
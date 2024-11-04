using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Components.Misc.FlowField;
using Utilities;
using Core.Misc.FlowField;
using Utilities.Extensions;
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
            this.RequireForUpdate<GridNodePresenterConfig>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<GridNodePresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var gridNodePresenterConfig = SystemAPI.GetSingleton<GridNodePresenterConfig>();
            if (!gridNodePresenterConfig.ShowGridLines) return;
            
            var flowFieldGridMap = SystemAPI.GetSingleton<FlowFieldGridMap>();

            int2 gridMapSize = new(flowFieldGridMap.MapWidth, flowFieldGridMap.GetMapHeight());

            GameObject gameObject = new("GridLineDrawer");
            GridLinesDrawer gridLinesDrawer = gameObject.AddComponent<GridLinesDrawer>();
            gridLinesDrawer.GridMapSize = gridMapSize;
            gridLinesDrawer.DrawCellRadius = 0.5f;
            gridLinesDrawer.GridOffset = flowFieldGridMap.GridOffset;
            gridLinesDrawer.DrawColor = Color.yellow;

        }


    }
}
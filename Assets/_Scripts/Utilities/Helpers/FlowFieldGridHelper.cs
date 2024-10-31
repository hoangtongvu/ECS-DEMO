using Components.Misc.FlowField;
using Core.Misc.FlowField;
using Core.UI.FlowField.GridNodePresenter;
using UnityEngine;
using Unity.Mathematics;
using Utilities.Extensions;

namespace Utilities.Helpers
{
    public static class FlowFieldGridHelper
    {
        public static float2 GetVectorFromDirection(SimpleDirection direction)
        {
            return direction switch
            {
                SimpleDirection.Top => new float2(0, 1),
                SimpleDirection.Bottom => new float2(0, -1),
                SimpleDirection.Left => new float2(-1, 0),
                SimpleDirection.Right => new float2(1, 0),
                SimpleDirection.TopLeft => math.normalize(new float2(-1, 1)),
                SimpleDirection.TopRight => math.normalize(new float2(1, 1)),
                SimpleDirection.BottomLeft => math.normalize(new float2(-1, -1)),
                SimpleDirection.BottomRight => math.normalize(new float2(1, -1)),
                _ => float2.zero,
            };

        }

        public static int2 MapIndexToGridPos(int gridMapWidth, int mapIndex)
        {
            int x = mapIndex % gridMapWidth;     // Column index
            int y = mapIndex / gridMapWidth;     // Row index
            return new int2(x, y);
        }

        public static int GridPosToMapIndex(int gridMapWidth, int2 gridPos) => GridPosToMapIndex(gridMapWidth, gridPos.x, gridPos.y);

        public static int GridPosToMapIndex(int gridMapWidth, int x, int y) => y * gridMapWidth + x;

        public static void SyncValuesToNodePresenter(
            in GridNodePresenterConfig presenterConfig
            , GridNodePresenterCtrl presenterCtrl
            , in FlowFieldGridNode node)
        {
            if (presenterConfig.ShowCost) presenterCtrl.CostText.SetCost(node.Cost);
            else presenterCtrl.CostText.Clear();

            if (presenterConfig.ShowBestCost) presenterCtrl.BestCostText.SetBestCost(node.BestCost);
            else presenterCtrl.BestCostText.Clear();

            if (presenterConfig.ShowDirectionVector) presenterCtrl.DirectionImage.SetDirection(node.DirectionVector);

            if (node.IsImpassable()) presenterCtrl.BackgroundImage.SetColor(Color.red);

        }

    }


}

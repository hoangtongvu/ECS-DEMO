using Core.Misc.FlowField;
using Unity.Mathematics;

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

        public static int2 MapIndexToGridPos(int gridWidth, int mapIndex)
        {
            int x = mapIndex % gridWidth;     // Column index
            int y = mapIndex / gridWidth;     // Row index
            return new int2(x, y);
        }

        public static int GridPosToMapIndex(int gridWidth, int2 gridPos)
        {
            return gridPos.y * gridWidth + gridPos.x;
        }


    }


}

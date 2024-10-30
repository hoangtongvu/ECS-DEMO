using Core.Misc.FlowField;
using Utilities.Helpers;
using Components.Misc.FlowField;

namespace Utilities.Extensions
{
    public static class FlowFieldGridMapExtension
    {
        public static int GetMapHeight(this FlowFieldGridMap map) => map.Nodes.Length / map.MapWidth;

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int x, int y)
        {
            int mapIndex = FlowFieldGridHelper.GridPosToMapIndex(map.MapWidth, new(x, y));
            return map.Nodes[mapIndex];
        }

    }


}

using Core.Misc.FlowField;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.FlowField
{
    public struct FlowFieldGridMap : IComponentData
    {
        public NativeArray<FlowFieldGridNode> Nodes;
        public int MapWidth;
    }

}

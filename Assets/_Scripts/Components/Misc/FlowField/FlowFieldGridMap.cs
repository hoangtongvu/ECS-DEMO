using Core.Misc.FlowField;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.FlowField
{
    public struct FlowFieldGridMap : IComponentData
    {
        public NativeArray<FlowFieldGridNode> Nodes;
        public int MapWidth;
        public int2 TargetGridPos;// Should we make this nullable?
        public int2 GridOffset;
    }

}

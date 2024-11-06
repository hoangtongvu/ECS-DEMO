using Unity.Entities;

namespace Components.Misc.FlowField
{
    public struct FlowFieldMapWidth : IComponentData
    {
        public int Value;
    }

    public struct FlowFieldMapHeight : IComponentData
    {
        public int Value;
    }

}

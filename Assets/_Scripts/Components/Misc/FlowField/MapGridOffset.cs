using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.FlowField
{
    public struct MapGridOffset : IComponentData
    {
        public int2 Value;
    }

}

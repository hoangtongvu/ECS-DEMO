using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.FlowField
{
    public struct TargetGridPos : IComponentData
    {
        public int2 Value;
    }

}

using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap
{
    public struct TargetCellPos : IComponentData
    {
        public int2 Value;
    }

}

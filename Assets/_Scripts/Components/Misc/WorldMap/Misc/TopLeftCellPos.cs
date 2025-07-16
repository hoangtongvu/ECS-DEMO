using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.Misc
{
    public struct TopLeftCellPos : IComponentData, ICleanupComponentData
    {
        public int2 Value;
    }
}

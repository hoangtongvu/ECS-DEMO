using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap
{
    public struct MapGridOffset : IComponentData
    {
        public int2 Value;
    }

}

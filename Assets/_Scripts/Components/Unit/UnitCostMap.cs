using Core.Unit;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit
{
    public struct UnitCostMap : IComponentData
    {
        public NativeHashMap<UnitCostId, uint> Value;
    }

    public struct LocalCostMapElement : IBufferElementData
    {
        public uint Cost;
    }

}

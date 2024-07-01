using Core.Unit;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit
{
    public struct UnitCostMap : IComponentData
    {
        public NativeHashMap<UnitCostId, uint> Value;
    }
}

using Core.Unit;
using Core.Unit.DarkUnit;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.DarkUnit
{
    public struct DarkUnitProfileMap : IComponentData
    {
        public NativeHashMap<UnitProfileId, DarkUnitProfileElement> Value;
    }
}

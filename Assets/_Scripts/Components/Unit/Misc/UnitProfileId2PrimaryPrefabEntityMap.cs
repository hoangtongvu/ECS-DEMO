using Core.Unit;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.Misc
{
    public struct UnitProfileId2PrimaryPrefabEntityMap : IComponentData
    {
        public NativeHashMap<UnitProfileId, Entity> Value;
    }

}

using Core.Unit;
using Core.Unit.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.Misc
{
    public struct AttackConfigsMap : IComponentData
    {
        public NativeHashMap<UnitProfileId, AttackConfigs> Value;
    }

}

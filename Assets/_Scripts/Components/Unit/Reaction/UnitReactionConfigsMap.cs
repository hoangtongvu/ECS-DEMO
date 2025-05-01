using Core.Unit;
using Core.Unit.Reaction;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.Reaction
{
    public struct UnitReactionConfigsMap : IComponentData
    {
        public NativeHashMap<UnitProfileId, UnitReactionConfigs> Value;
    }

}

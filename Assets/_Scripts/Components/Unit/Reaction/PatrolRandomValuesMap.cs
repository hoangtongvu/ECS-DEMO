using Core.Unit.Reaction;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.Reaction
{
    public struct PatrolRandomValuesMap : IComponentData
    {
        public NativeHashMap<Entity, PatrolRandomValues> Value;
    }

}

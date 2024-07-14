using Core.Unit;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit
{
    public struct MoveAffecterMap : IComponentData
    {
        public NativeHashMap<MoveAffecterId, byte> Value;
    }

}

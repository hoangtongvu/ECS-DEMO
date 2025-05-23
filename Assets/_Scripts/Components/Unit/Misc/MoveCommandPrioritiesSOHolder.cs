using Core.Unit.Misc;
using Unity.Entities;

namespace Components.Unit.Misc
{
    public struct MoveCommandPrioritiesSOHolder : IComponentData
    {
        public UnityObjectRef<MoveCommandPrioritiesSO> Value;
    }

}

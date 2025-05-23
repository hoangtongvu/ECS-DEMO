using Core.Unit.MyMoveCommand;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.Misc
{
    public struct MoveCommandPrioritiesMap : IComponentData
    {
        public NativeArray<MoveCommandSource> Value;
    }

}

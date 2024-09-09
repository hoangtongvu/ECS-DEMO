using Core.Unit.MyMoveCommand;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.MyMoveCommand
{
    public struct MoveCommandSourceMap : IComponentData
    {
        public NativeHashMap<MoveCommandSourceId, byte> Value;
    }

}

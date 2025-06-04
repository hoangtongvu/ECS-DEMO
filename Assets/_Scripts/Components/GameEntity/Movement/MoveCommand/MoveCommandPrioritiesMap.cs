using Core.GameEntity.Movement.MoveCommand;
using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity.Movement.MoveCommand
{
    public struct MoveCommandPrioritiesMap : IComponentData
    {
        public NativeArray<MoveCommandSource> Value;
    }

}

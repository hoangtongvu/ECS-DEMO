using Core.GameEntity.Movement.MoveCommand;
using Unity.Entities;

namespace Components.GameEntity.Movement.MoveCommand
{
    public struct MoveCommandPrioritiesSOHolder : IComponentData
    {
        public UnityObjectRef<MoveCommandPrioritiesSO> Value;
    }

}

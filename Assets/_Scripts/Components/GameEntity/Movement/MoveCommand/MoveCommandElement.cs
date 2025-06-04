using Core.GameEntity.Movement.MoveCommand;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Movement.MoveCommand
{
    public struct MoveCommandElement : IComponentData
    {
        public MoveCommandSource CommandSource;
        public float3 Float3;
        public Entity TargetEntity;
    }

}

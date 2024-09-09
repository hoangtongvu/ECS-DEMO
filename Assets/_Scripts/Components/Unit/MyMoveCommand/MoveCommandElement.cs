using Core.Unit.MyMoveCommand;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit.MyMoveCommand
{
    public struct MoveCommandElement : IComponentData
    {
        public MoveCommandSource CommandSource;
        public float3 Float3;
        public Entity TargetEntity;
    }

}

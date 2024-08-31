using Core.Unit.MyMoveCommand;
using Unity.Entities;

namespace Components.Unit.MyMoveCommand
{
    public struct MoveCommandICD : IComponentData
    {
        public MoveCommand Value;
    }

}

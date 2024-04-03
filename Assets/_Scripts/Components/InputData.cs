using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MoveDirectionFloat2
    {
        public float2 Value;
    }

    public struct InputData : IComponentData
    {

        public MoveDirectionFloat2 MoveDirection;
        public bool LeftMouseDown;
        public bool RightMouseDown;
        public bool BackspaceButtonDown;

    }


}

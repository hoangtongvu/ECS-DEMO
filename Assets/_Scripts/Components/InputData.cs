using Unity.Entities;

namespace Components
{

    public struct InputData : IComponentData
    {

        public MoveDirectionFloat2 MoveDirection;
        public bool LeftMouseDown;
        public bool RightMouseDown;
        public bool BackspaceButtonDown;

        public MouseData LeftMouseData;
        public MouseData RightMouseData;
    }

    public struct MouseData
    {
        public bool Down;
        public bool Hold;
        public bool Up;
    }

}

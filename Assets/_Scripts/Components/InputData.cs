using Unity.Entities;

namespace Components
{

    public struct InputData : IComponentData
    {

        public MoveDirectionFloat2 MoveDirection;
        public bool BackspaceButtonDown;
        public bool SpaceButtonDown;
        public bool EnterButtonDown;

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

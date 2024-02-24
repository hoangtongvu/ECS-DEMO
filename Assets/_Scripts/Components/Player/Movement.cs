using Unity.Entities;

namespace Components.Player
{
    public struct MoveDirection : IComponentData, IEnableableComponent
    {
        public bool Up;
        public bool Down;
        public bool Left;
        public bool Right;
    }

    public struct MoveSpeed : IComponentData, IEnableableComponent
    {
        public float Value;
    }
    
}

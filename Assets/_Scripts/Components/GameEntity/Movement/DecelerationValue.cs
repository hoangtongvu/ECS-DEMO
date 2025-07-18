using Unity.Entities;

namespace Components.GameEntity.Movement
{
    public struct DecelerationValue : ISharedComponentData
    {
        public float Value;
    }
}

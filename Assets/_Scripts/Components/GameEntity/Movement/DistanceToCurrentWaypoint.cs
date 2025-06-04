using Unity.Entities;

namespace Components.GameEntity.Movement
{
    public struct DistanceToCurrentWaypoint : IComponentData
    {
        public float Value;
    }

}

using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Movement
{
    public struct CurrentWorldWaypoint : IComponentData
    {
        public float3 Value;
    }

}

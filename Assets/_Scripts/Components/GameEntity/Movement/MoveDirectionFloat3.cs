using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Movement
{
    public struct MoveDirectionFloat3 : IComponentData
    {
        public float3 Value;
    }
}

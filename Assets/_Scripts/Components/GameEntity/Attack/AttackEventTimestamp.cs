using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Attack
{
    public struct AttackEventTimestamp : IComponentData
    {
        public half Value;
    }
}

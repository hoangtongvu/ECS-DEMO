using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Attack
{
    public struct AttackDurationSeconds : IComponentData
    {
        public half Value;
    }
}

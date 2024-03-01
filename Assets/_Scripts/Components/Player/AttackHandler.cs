using Unity.Entities;
using Unity.Mathematics;

namespace Components.Player
{
    public struct AttackData : IComponentData, IEnableableComponent
    {
        public bool isAttacking;
        public float attackTimeCounter;
        public float attackDurationSecond;
    }

    public struct HitBox : IComponentData, IEnableableComponent
    {
        public float3 HitBoxPos;
        public float3 HitBoxSize;
    }

}

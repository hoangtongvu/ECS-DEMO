using Unity.Entities;
using Unity.Mathematics;

namespace Components.Player
{

    public struct AttackInput : IComponentData, IEnableableComponent
    {
        public bool IsAttackable;
    }

    public struct AttackData : IComponentData, IEnableableComponent
    {
        public bool isAttacking;
        public float attackTimeCounter;
        public float attackDurationSecond;
    }

    public struct HitBox : IComponentData, IEnableableComponent
    {
        public float3 HitBoxLocalPos;
        public float3 HitBoxSize;
    }

}

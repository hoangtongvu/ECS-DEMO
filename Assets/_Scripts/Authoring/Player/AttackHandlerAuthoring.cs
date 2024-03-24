using Components.Player;
using Core.Animator;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
    public class AttackHandlerAuthoring : MonoBehaviour
    {
        //public BaseAnimator animator;
        public Vector3 HitBoxPos;
        public Vector3 HitBoxSize = new(1f, 1f, 1f);

        private class Baker : Baker<AttackHandlerAuthoring>
        {
            public override void Bake(AttackHandlerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);


                AddComponent(entity, new AttackData
                {
                    isAttacking = false,
                    attackTimeCounter = 0f,
                    attackDurationSecond = 0.667f,
                    /*Note This currently typed manually cause I can't reference animator into Authoring. 
                    //it should equal this: authoring.animator.GetAnimationLength("Punching"),
                    AttackSystem set this attackDurationSecond for now.*/
                });

                AddComponent(entity, new AttackInput
                {
                    IsAttackable = false,
                });

                AddComponent(entity, new HitBox
                {
                    HitBoxLocalPos = authoring.HitBoxPos,
                    HitBoxSize = authoring.HitBoxSize,
                });
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position + this.HitBoxPos, this.HitBoxSize);
        }

    }
}

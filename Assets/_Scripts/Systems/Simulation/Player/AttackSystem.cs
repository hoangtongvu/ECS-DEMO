using Unity.Entities;
using UnityEngine;
using Components.Damage;
using Core.Animator;
using Components;
using Components.Player;


namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackSystem : SystemBase
    {

        private const string ATTACK_ANIM_NAME = "Punching";
        private const string IDLE_ANIM_NAME = "Idle";


        protected override void OnCreate()
        {
            this.RequireForUpdate<DmgValue>();
        }

        protected override void OnUpdate()
        {
            
            foreach (var (attackDataRef, clipInfos) in
                SystemAPI.Query<
                    RefRW<AttackData>
                    , DynamicBuffer<AnimationClipInfoElement>>()
                    .WithAll<PlayerTag>())
            {

                if (clipInfos.IsEmpty) continue;

                if (attackDataRef.ValueRO.isAttacking)
                {
                    this.UpdateAttackTimeCounter(ref attackDataRef.ValueRW.attackTimeCounter);
                    if (this.AttackDurationEnded(attackDataRef))
                    {
                        attackDataRef.ValueRW.isAttacking = false;
                        attackDataRef.ValueRW.attackTimeCounter = 0f;
                        this.BackToIdleState();
                    }
                }
                

                if (!this.IsAttackable(attackDataRef)) return;
                this.Attack(attackDataRef);

            }


        }


        private bool IsAttackable(RefRW<AttackData> attackDataRef) => Input.GetMouseButtonDown(0) && !attackDataRef.ValueRO.isAttacking;

        private void Attack(RefRW<AttackData> attackDataRef)
        {
            //Play Attack Animation.
            attackDataRef.ValueRW.isAttacking = true;
            foreach (var animatorDataRef in SystemAPI.Query<RefRW<AnimatorData>>().WithAll<PlayerTag>())
            {
                AnimatorHelper.TryChangeAnimatorData(animatorDataRef, ATTACK_ANIM_NAME);
            }
            Debug.Log("Attack!");
            
        }

        private void UpdateAttackTimeCounter(ref float attackTimeCounter) =>
            attackTimeCounter += SystemAPI.Time.DeltaTime;


        private bool AttackDurationEnded(RefRW<AttackData> attackDataRef) =>
            attackDataRef.ValueRO.attackTimeCounter >= attackDataRef.ValueRO.attackDurationSecond;

        private void BackToIdleState() // Temporary code.
        {
            foreach (var animatorDataRef in SystemAPI.Query<RefRW<AnimatorData>>().WithAll<PlayerTag>())
            {
                AnimatorHelper.TryChangeAnimatorData(animatorDataRef, IDLE_ANIM_NAME);
            }
        }

    }
}


using Unity.Entities;
using UnityEngine;
using Components.Damage;
using Core.Animator;
using Components;
using Components.CustomIdentification;
using Components.Player;


namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackSystem : SystemBase
    {
        private BaseAnimator baseAnimator;
        private const string ATTACK_ANIM_NAME = "Punching";
        private const string IDLE_ANIM_NAME = "Idle";


        protected override void OnCreate()
        {
            this.RequireForUpdate<DmgValue>();
        }

        protected override void OnUpdate()
        {
            
            foreach (var attackDataRef in 
                SystemAPI.Query<RefRW<AttackData>>().WithAll<PlayerTag>())
            {

                if (this.baseAnimator == null) // TODO: Find another way to SetBaseAnimator.
                {
                    this.SetBaseAnimator();
                    this.SetAttackDuration(out attackDataRef.ValueRW.attackDurationSecond);
                }

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

        private void SetBaseAnimator() // Used to set Player's BaseAnimator.
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton<BaseAnimatorMap>(out var baseAnimatorMap))
            {
                Debug.LogError("BaseAnimatorMap Singleton not found");
                return;
            }

            foreach (var idRef in SystemAPI.Query<RefRO<UniqueId>>())
            {
                if (idRef.ValueRO.Kind != UniqueKind.Player) continue;

                if (baseAnimatorMap.Value.TryGetValue(idRef.ValueRO, out this.baseAnimator)) return;
                Debug.LogError($"Can't get BaseAnimator with {idRef.ValueRO} in BaseAnimatorMap");
                
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


        // TODO: Add a Dynamic Buffer that store (anim Name + anim Length) to remove access BaseAnimator directly.
        private void SetAttackDuration(out float attackDurationSecond) =>
            attackDurationSecond = this.baseAnimator.GetAnimationLength(ATTACK_ANIM_NAME);

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


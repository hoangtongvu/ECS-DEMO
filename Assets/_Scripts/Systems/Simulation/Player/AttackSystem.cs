using Unity.Entities;
using UnityEngine;
using Components.Damage;
using Core.Animator;
using Components;
using Components.CustomIdentification;
using Components.Player;
using Unity.Physics.Aspects;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackSystem : SystemBase
    {
        private BaseAnimator animator;


        protected override void OnCreate()
        {
            this.RequireForUpdate<DmgValue>();
        }

        protected override void OnUpdate()
        {

            foreach (var attackDataRef in 
                SystemAPI.Query<RefRW<AttackData>>().WithAll<PlayerTag>())
            {

                if (this.animator == null) // TODO: Find another way to SetBaseAnimator.
                {
                    this.SetBaseAnimator();
                    this.SetAttackDuration(attackDataRef);
                }

                if (attackDataRef.ValueRO.isAttacking)
                {
                    this.UpdateAttackTimeCounter(attackDataRef);
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

        private void SetBaseAnimator()
        {
            if (!SystemAPI.ManagedAPI.TryGetSingleton<UnityObjectMap>(out var objectMap))
            {
                Debug.LogError("UnityObjectMap Singleton not found");
                return;
            }

            foreach (var idRef in
                SystemAPI.Query<RefRO<UniqueId>>())
            {
                if (idRef.ValueRO.Kind != UniqueKind.Player) continue;
                
                if (!objectMap.Value.TryGetValue(idRef.ValueRO, out UnityEngine.Object unityObj))
                {
                    Debug.LogError($"Can't get Unity Obj with {idRef.ValueRO} in UnityObjMap");
                    return;
                }

                this.animator = ((GameObject) unityObj).GetComponent<BaseAnimator>();
            }
        }

        private bool IsAttackable(RefRW<AttackData> attackDataRef) => Input.GetMouseButtonDown(0) && !attackDataRef.ValueRO.isAttacking;

        private void Attack(RefRW<AttackData> attackDataRef)
        {
            //Play Attack Animation.
            attackDataRef.ValueRW.isAttacking = true;
            this.animator.ChangeAnimationState("Punching");
            Debug.Log("Attack!");
            
        }

        private void UpdateAttackTimeCounter(RefRW<AttackData> attackDataRef) =>
            attackDataRef.ValueRW.attackTimeCounter += SystemAPI.Time.DeltaTime;

        private void SetAttackDuration(RefRW<AttackData> attackDataRef) =>
            attackDataRef.ValueRW.attackDurationSecond = this.animator.GetAnimationLength("Punching");

        private bool AttackDurationEnded(RefRW<AttackData> attackDataRef) =>
            attackDataRef.ValueRO.attackTimeCounter >= attackDataRef.ValueRO.attackDurationSecond;

        private void BackToIdleState() => this.animator.ChangeAnimationState("Idle");

    }
}


using Unity.Entities;
using Components.Damage;
using Components;
using Components.Player;
using Core;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackSystem : SystemBase //TODO: This can be changed into ISystem + burst.
    {

        private const string ATTACK_ANIM_NAME = "Punching";
        private const string IDLE_ANIM_NAME = "Idle";


        protected override void OnCreate()
        {
            this.RequireForUpdate<LocalTransform>();
            this.RequireForUpdate<HitBox>();
            this.RequireForUpdate<DmgValue>();
            this.RequireForUpdate<AttackData>();
            this.RequireForUpdate<AttackInput>();
            this.RequireForUpdate<AnimationClipInfoElement>();
            this.RequireForUpdate<PlayerTag>();
            this.RequireForUpdate<AnimatorData>();
        }

        protected override void OnUpdate()
        {

            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();


            foreach (var (transformRef, hitboxRef, dmgValueRef, attackDataRef, attackInputRef, clipInfos) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRO<HitBox>
                    , RefRO<DmgValue>
                    , RefRW<AttackData>
                    , RefRO<AttackInput>
                    , DynamicBuffer<AnimationClipInfoElement>>()
                    .WithAll<PlayerTag>())
            {

                if (clipInfos.IsEmpty)
                {
                    UnityEngine.Debug.LogError("clipInfos.IsEmpty");
                    continue;
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


                if (!attackInputRef.ValueRO.IsAttackable) return;
                this.Attack(attackDataRef);
                this.TryCatchColliderInHitbox(physicsWorld, hitboxRef, transformRef, dmgValueRef);
            }

        }


        private void Attack(RefRW<AttackData> attackDataRef)
        {
            //Play Attack Animation.
            attackDataRef.ValueRW.isAttacking = true;

            foreach (var animatorDataRef in SystemAPI.Query<RefRW<AnimatorData>>().WithAll<PlayerTag>())
            {
                animatorDataRef.ValueRW.Value.ChangeValue(ATTACK_ANIM_NAME);
            }

        }

        private void TryCatchColliderInHitbox(
            in PhysicsWorldSingleton physicsWorld
            , RefRO<HitBox> hitboxRef
            , RefRO<LocalTransform> transformRef
            , RefRO<DmgValue> dmgValueRef)
        {
            
            NativeList<DistanceHit> hits = new(Allocator.Temp);

            physicsWorld.OverlapBox(
                transformRef.ValueRO.Position + hitboxRef.ValueRO.HitBoxLocalPos,
                quaternion.identity,
                hitboxRef.ValueRO.HitBoxSize / 2,
                ref hits,
                new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Player,
                    CollidesWith = (uint)CollisionLayer.Default,
                });

            foreach (var hit in hits)
            {
                Entity entity = hit.Entity;


                if (!SystemAPI.HasComponent<HpChangeState>(entity)) continue;
                RefRW<HpChangeState> hpChangeStateRef = SystemAPI.GetComponentRW<HpChangeState>(entity);
                HpChangeHandleSystem.DeductHp(ref hpChangeStateRef.ValueRW, dmgValueRef.ValueRO.Value);
                // Debug.Log($"{entity} lost {dmgValueRef.ValueRO.Value} Hp.");
            }

            hits.Dispose();
            
        }


        private void UpdateAttackTimeCounter(ref float attackTimeCounter) =>
            attackTimeCounter += SystemAPI.Time.DeltaTime;


        private bool AttackDurationEnded(RefRW<AttackData> attackDataRef) =>
            attackDataRef.ValueRO.attackTimeCounter >= attackDataRef.ValueRO.attackDurationSecond;

        private void BackToIdleState() // Temporary code.
        {

            foreach (var animatorDataRef in SystemAPI.Query<RefRW<AnimatorData>>().WithAll<PlayerTag>())
            {
                animatorDataRef.ValueRW.Value.ChangeValue(IDLE_ANIM_NAME);
            }

        }

    }
}

using Unity.Entities;
using Components.GameEntity.Damage;
using Components.Player;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Utilities.Extensions;
using Utilities.Extensions.GameEntity.Damage;
using Components.Misc;
using Core.Misc;

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
            this.RequireForUpdate<PlayerTag>();
            this.RequireForUpdate<AnimatorData>();
        }

        protected override void OnUpdate()
        {
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (transformRef, hitboxRef, dmgValueRef, attackDataRef, attackInputRef) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRO<HitBox>
                    , RefRO<DmgValue>
                    , RefRW<AttackData>
                    , RefRO<AttackInput>>()
                    .WithAll<PlayerTag>())
            {
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

                if (!SystemAPI.HasComponent<CurrentHp>(entity)) continue;


                var hpChangeRecords = SystemAPI.GetBuffer<HpChangeRecordElement>(entity);
                hpChangeRecords.AddDeductRecord(dmgValueRef.ValueRO.Value);

                //UnityEngine.Debug.Log($"{entity} received {dmgValueRef.ValueRO.Value} Dmg.");
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

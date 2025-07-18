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
using Components.GameEntity.Misc;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackSystem : SystemBase //TODO: This can be changed into ISystem + burst.
    {
        private const string ATTACK_ANIM_NAME = "2H_Melee_Attack_Slice";
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

            foreach (var (transformRef, lookDirXZRef, dmgValueRef, attackDataRef, attackInputRef, entity) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , RefRO<LookDirectionXZ>
                    , RefRO<DmgValue>
                    , RefRW<AttackData>
                    , RefRO<AttackInput>>()
                .WithAll<
                    PlayerTag>()
                .WithEntityAccess())
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
                this.Attack(attackDataRef, in entity);
                this.TryCatchColliderInHitbox(physicsWorld, in lookDirXZRef.ValueRO, in transformRef.ValueRO, in dmgValueRef.ValueRO, in entity);

            }

        }

        private void Attack(RefRW<AttackData> attackDataRef, in Entity entity)
        {
            //Play Attack Animation.
            attackDataRef.ValueRW.isAttacking = true;

            foreach (var animatorDataRef in SystemAPI.Query<RefRW<AnimatorData>>().WithAll<PlayerTag>())
            {
                animatorDataRef.ValueRW.Value.ChangeValue(ATTACK_ANIM_NAME);
            }

            SystemAPI.SetComponentEnabled<InAttackStateTag>(entity, true);
            SystemAPI.SetComponent(entity, new InAttackStateTimeStamp
            {
                Value = SystemAPI.Time.ElapsedTime,
            });

        }

        private void TryCatchColliderInHitbox(
            in PhysicsWorldSingleton physicsWorld
            , in LookDirectionXZ lookDirectionXZ
            , in LocalTransform transform
            , in DmgValue dmgValue
            , in Entity playerEntity)
        {
            NativeList<DistanceHit> hits = new(Allocator.Temp);

            const float hitBoxRadius = 1.0f; // TODO: Find this value else where

            physicsWorld.OverlapSphere(
                transform.Position + new float3(lookDirectionXZ.Value.x, 0f, lookDirectionXZ.Value.y)
                , hitBoxRadius
                , ref hits
                , new()
                {
                    BelongsTo = (uint)CollisionLayer.Player,
                    CollidesWith = (uint)(CollisionLayerConstants.Damagable | CollisionLayer.Default),
                });

            foreach (var hit in hits)
            {
                Entity entity = hit.Entity;
                if (entity == playerEntity) continue;
                if (!SystemAPI.HasComponent<CurrentHp>(entity)) continue;

                var hpChangeRecords = SystemAPI.GetBuffer<HpChangeRecordElement>(entity);
                hpChangeRecords.AddDeductRecord(dmgValue.Value);
            }

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

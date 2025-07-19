using Unity.Entities;
using Components.GameEntity.Damage;
using Components.Player;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Transforms;
using Utilities.Extensions.GameEntity.Damage;
using Core.Misc;
using Unity.Burst;
using Components.GameEntity.Misc;
using Components.GameEntity.Reaction;
using Components.GameEntity.Attack;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct DealDmgSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , LookDirectionXZ
                    , DmgValue
                    , AttackReaction.TimerSeconds
                    , AttackEventTimestamp>()
                .WithAll<
                    AttackReaction.UpdatingTag>()
                .WithAll<
                    AttackEventTriggeredTag>()
                .WithAll<
                    PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (transformRef, lookDirXZRef, dmgValueRef, attackTimerSecondsRef, attackEventTimestampRef, entity) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , RefRO<LookDirectionXZ>
                    , RefRO<DmgValue>
                    , RefRO<AttackReaction.TimerSeconds>
                    , RefRO<AttackEventTimestamp>>()
                .WithAll<
                    AttackReaction.UpdatingTag>()
                .WithDisabled<
                    AttackEventTriggeredTag>()
                .WithAll<
                    PlayerTag>()
                .WithEntityAccess())
            {
                if (attackTimerSecondsRef.ValueRO.Value < attackEventTimestampRef.ValueRO.Value) continue;

                SystemAPI.SetComponentEnabled<AttackEventTriggeredTag>(entity, true);
                SystemAPI.SetComponentEnabled<InAttackStateTag>(entity, true);
                SystemAPI.SetComponent(entity, new InAttackStateTimeStamp
                {
                    Value = SystemAPI.Time.ElapsedTime,
                });

                this.DealDmg(ref state, physicsWorld, in lookDirXZRef.ValueRO, in transformRef.ValueRO, in dmgValueRef.ValueRO, in entity);

            }

        }

        [BurstCompile]
        private void DealDmg(
            ref SystemState state
            , in PhysicsWorldSingleton physicsWorld
            , in LookDirectionXZ lookDirectionXZ
            , in LocalTransform transform
            , in DmgValue dmgValue
            , in Entity playerEntity)
        {
            NativeList<DistanceHit> hits = new(Allocator.Temp);

            const float hitBoxRadius = 1.7f; // TODO: Find this value else where

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

    }

}

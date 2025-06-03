using Components;
using Components.Damage;
using Components.GameEntity.Interaction;
using Components.Unit;
using Components.Unit.Reaction;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities.Extensions;

namespace Systems.Simulation.Unit.Reaction
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial struct WorkReactionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    WorkStartedTag
                    , InteractingEntity
                    , IsUnitWorkingTag
                    , IsAliveTag
                    , AnimatorData>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (workStartedTag, interactingEntityRef, isUnitWorkingTag, isAliveTag, animatorDataRef, transformRef) in
                SystemAPI.Query<
                    EnabledRefRW<WorkStartedTag>
                    , RefRO<InteractingEntity>
                    , EnabledRefRW<IsUnitWorkingTag>
                    , EnabledRefRO<IsAliveTag>
                    , RefRW<AnimatorData>
                    , RefRW<LocalTransform>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool workTargetValid = interactingEntityRef.ValueRO.Value != Entity.Null;
                bool canUpdate = isAliveTag.ValueRO && workTargetValid;
                bool reactionStarted = workStartedTag.ValueRO;

                if (canUpdate)
                {
                    if (!reactionStarted)
                    {
                        //UnityEngine.Debug.Log("Start");
                        isUnitWorkingTag.ValueRW = true;
                        animatorDataRef.ValueRW.Value.ChangeValue("1H_Melee_Attack_Chop");
                        workStartedTag.ValueRW = true;
                        this.RotateTowardInteractingEntity(ref state, ref transformRef.ValueRW, in interactingEntityRef.ValueRO.Value);
                    }

                    //UnityEngine.Debug.Log("Update");

                }

                if (!canUpdate && reactionStarted)
                {
                    //UnityEngine.Debug.Log("End");
                    isUnitWorkingTag.ValueRW = false;
                    workStartedTag.ValueRW = false;
                }

            }

        }

        [BurstCompile]
        private void RotateTowardInteractingEntity(
            ref SystemState state
            , ref LocalTransform transform
            , in Entity interactingEntity)
        {
            float3 targetPosition = SystemAPI.GetComponent<LocalTransform>(interactingEntity).Position;
            float3 direction = math.normalize(targetPosition - transform.Position);

            // Convert direction into rotation facing that direction
            quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());

            // Apply to entity
            transform.Rotation = targetRotation;
        }

    }

}
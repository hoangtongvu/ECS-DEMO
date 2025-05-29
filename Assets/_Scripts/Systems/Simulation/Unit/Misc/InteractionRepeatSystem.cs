using Components.Damage;
using Components.GameEntity;
using Components.Unit;
using Components.Unit.Misc;
using Core.GameEntity;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))] // TODO: Change this later on, we don't need checking repeatable this frequently. Might create a new system group that update every 1 second.
    [BurstCompile]
    public partial struct InteractionRepeatSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , InteractingEntity
                    , InteractionTypeICD
                    , MaxFollowDistance
                    , InteractableDistanceRange
                    , CanCheckInteractionRepeatTag>()
                .WithAll<
                    IsAliveTag
                    , IsUnitWorkingTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transformRef, interactingEntityRef, interactionTypeICDRef, maxFollowDistanceRef, interactableRangeRef, targetEntityWorldSquareRadiusRef, canCheckInteractionRepeatTag) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRW<InteractingEntity>
                    , RefRW<InteractionTypeICD>
                    , RefRO<MaxFollowDistance>
                    , RefRO<InteractableDistanceRange>
                    , RefRO<TargetEntityWorldSquareRadius>
                    , EnabledRefRW<CanCheckInteractionRepeatTag>>()
                    .WithAll<
                        IsAliveTag
                        , IsUnitWorkingTag>())
            {
                canCheckInteractionRepeatTag.ValueRW = false;

                if (!SystemAPI.Exists(interactingEntityRef.ValueRO.Value))
                {
                    this.StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                float currentDistance = -targetEntityWorldSquareRadiusRef.ValueRO.Value + this.GetCurrentDistance2(
                    ref state
                    , in transformRef.ValueRO.Position
                    , in interactingEntityRef.ValueRO.Value);

                if (currentDistance > maxFollowDistanceRef.ValueRO.Value)
                {
                    this.StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                bool isCurrentDistanceInInteractableRange =
                    currentDistance >= interactableRangeRef.ValueRO.MinValue &&
                    currentDistance <= interactableRangeRef.ValueRO.MaxValue;

                if (!isCurrentDistanceInInteractableRange)
                {
                    this.StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

            }

        }

        [BurstCompile]
        private float GetCurrentDistance2(
            ref SystemState state
            , in float3 unitPos
            , in Entity interactingEntity)
        {
            float3 interactingEntityPos = SystemAPI.GetComponent<LocalTransform>(interactingEntity).Position;
            return math.sqrt(math.square(unitPos.x - interactingEntityPos.x) + math.square(unitPos.z - interactingEntityPos.z));
        }

        [BurstCompile]
        private void StopInteraction(
            ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD)
        {
            interactingEntity.Value = Entity.Null;
            interactionTypeICD.Value = InteractionType.None;
        }

    }

}
using Components.GameEntity;
using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static Utilities.Helpers.Misc.InteractionHelper;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class InteractionRepeatSystemGroup : ComponentSystemGroup
    {
        public InteractionRepeatSystemGroup()
        {
            this.RateManager = new RateUtils.VariableRateManager(500);
        }

    }

    [UpdateInGroup(typeof(InteractionRepeatSystemGroup))]
    [BurstCompile]
    public partial struct InteractionRepeatSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , LocalTransform
                    , InteractingEntity
                    , InteractionTypeICD
                    , InteractableDistanceRange
                    , TargetEntityWorldSquareRadius
                    , CanCheckInteractionRepeatTag>()
                .WithAll<
                    IsAliveTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var maxFollowDistanceMap = SystemAPI.GetSingleton<MaxFollowDistanceMap>().Value;

            foreach (var (unitProfileIdRef, transformRef, interactingEntityRef, interactionTypeICDRef, interactableRangeRef, targetEntityWorldSquareRadiusRef, canCheckInteractionRepeatTag) in
                SystemAPI.Query<
                    RefRO<UnitProfileIdHolder>
                    , RefRO<LocalTransform>
                    , RefRW<InteractingEntity>
                    , RefRW<InteractionTypeICD>
                    , RefRO<InteractableDistanceRange>
                    , RefRO<TargetEntityWorldSquareRadius>
                    , EnabledRefRW<CanCheckInteractionRepeatTag>>()
                    .WithAll<
                        IsAliveTag>())
            {
                canCheckInteractionRepeatTag.ValueRW = false;

                if (!SystemAPI.Exists(interactingEntityRef.ValueRO.Value))
                {
                    StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                float currentDistance = -targetEntityWorldSquareRadiusRef.ValueRO.Value + this.GetEuclideanDistance2(
                    ref state
                    , in transformRef.ValueRO.Position
                    , in interactingEntityRef.ValueRO.Value);

                float2 currentAbsDistanceXZ = this.GetAbsoluteDistanceXZ(
                    ref state
                    , in transformRef.ValueRO.Position
                    , in interactingEntityRef.ValueRO.Value);

                currentAbsDistanceXZ -= new float2(targetEntityWorldSquareRadiusRef.ValueRO.Value);
                currentAbsDistanceXZ = math.abs(currentAbsDistanceXZ);

                if (currentDistance > maxFollowDistanceMap[unitProfileIdRef.ValueRO.Value])
                {
                    StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                bool isCurrentDistanceInInteractableRange = this.IsAbsDistanceXZInInteractableRange(
                    in currentAbsDistanceXZ
                    , in interactableRangeRef.ValueRO);

                if (!isCurrentDistanceInInteractableRange)
                {
                    StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

            }

        }

        [BurstCompile]
        private float GetEuclideanDistance2(
            ref SystemState state
            , in float3 unitPos
            , in Entity interactingEntity)
        {
            float3 interactingEntityPos = SystemAPI.GetComponent<LocalTransform>(interactingEntity).Position;
            return math.sqrt(math.square(unitPos.x - interactingEntityPos.x) + math.square(unitPos.z - interactingEntityPos.z));
        }

        [BurstCompile]
        private float2 GetAbsoluteDistanceXZ(
            ref SystemState state
            , in float3 unitPos
            , in Entity interactingEntity)
        {
            float3 interactingEntityPos = SystemAPI.GetComponent<LocalTransform>(interactingEntity).Position;
            return math.abs(new float2(interactingEntityPos.x - unitPos.x, interactingEntityPos.z - unitPos.z));
        }

        [BurstCompile]
        private bool IsAbsDistanceXZInInteractableRange(
            in float2 absDistanceXZ
            , in InteractableDistanceRange interactableDistanceRange)
        {
            // (min <= x <= max && 0 <= z <= max) || (0 <= x <= min && min <= z <= max)

            bool firstCondition = absDistanceXZ.x <= interactableDistanceRange.MaxValue
                && absDistanceXZ.x >= interactableDistanceRange.MinValue
                && absDistanceXZ.y <= interactableDistanceRange.MaxValue
                && absDistanceXZ.y >= 0;

            if (firstCondition) return true;

            return absDistanceXZ.x <= interactableDistanceRange.MinValue
                && absDistanceXZ.x >= 0
                && absDistanceXZ.y <= interactableDistanceRange.MaxValue
                && absDistanceXZ.y >= interactableDistanceRange.MinValue;
        }

    }

}
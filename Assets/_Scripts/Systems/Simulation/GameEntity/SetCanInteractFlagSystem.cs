using Components.GameEntity;
using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Core.GameEntity.Movement.MoveCommand;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetCanInteractFlagSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    AbsoluteDistanceXZToTarget
                    , TargetEntity
                    , TargetEntityWorldSquareRadius
                    , InteractableDistanceRange
                    , CanMoveEntityTag
                    , MoveCommandElement
                    , CanInteractEntityTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Clear components.
            foreach (var (tagRef, targetEntityRef) in SystemAPI.Query<EnabledRefRW<CanInteractEntityTag>, RefRW<TargetEntity>>())
            {
                tagRef.ValueRW = false;
                targetEntityRef.ValueRW.Value = Entity.Null;
            }

            foreach (var (absoluteDistanceXZToTargetRef, targetEntityRef, targetEntityWorldSquareRadiusRef, interactableDistanceRangeRef, moveCommandElementRef, entity) in SystemAPI
                .Query<
                    RefRO<AbsoluteDistanceXZToTarget>
                    , RefRO<TargetEntity>
                    , RefRO<TargetEntityWorldSquareRadius>
                    , RefRO<InteractableDistanceRange>
                    , RefRW<MoveCommandElement>>()
                .WithDisabled<CanInteractEntityTag>()
                .WithEntityAccess())
        {
                if (targetEntityRef.ValueRO.Value == Entity.Null) continue;

                // NOTE: addValue = MaxDistanceRange -> Stay too close to upper bound of interactable range -> Possibly lose interaction
                // addValue = lerp(MinDistanceRange, MaxDistanceRange, 0.5f) -> Possibly move stopped before interaction occurred (if the interactableDistanceRange is between celRadius * 2)
                // -> addValue = lerp(MinDistanceRange, MaxDistanceRange, 0.75f) is the best possible value
                const float centerInteractableDistanceRatio = 0.75f;
                float addValue =
                    math.lerp(interactableDistanceRangeRef.ValueRO.MinValue, interactableDistanceRangeRef.ValueRO.MaxValue, centerInteractableDistanceRatio);

                float interactRadius = targetEntityWorldSquareRadiusRef.ValueRO.Value + addValue;

                if (absoluteDistanceXZToTargetRef.ValueRO.X > interactRadius) continue;
                if (absoluteDistanceXZToTargetRef.ValueRO.Z > interactRadius) continue;

                this.StopMove(ref state, ref moveCommandElementRef.ValueRW, in entity);

                SystemAPI.SetComponentEnabled<CanInteractEntityTag>(entity, true);

            }

        }

        [BurstCompile]
        private void StopMove(
            ref SystemState state
            , ref MoveCommandElement moveCommandElement
            , in Entity entity)
        {
            SystemAPI.SetComponentEnabled<CanMoveEntityTag>(entity, false);
            moveCommandElement.CommandSource = MoveCommandSource.None;
            moveCommandElement.TargetEntity = Entity.Null;
        }

    }

}
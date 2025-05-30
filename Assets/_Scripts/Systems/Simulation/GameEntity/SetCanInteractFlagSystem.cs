using Components;
using Components.GameEntity;
using Components.Unit.Misc;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Entities;

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

            foreach (var (absoluteDistanceXZToTargetRef, targetEntityRef, targetEntityWorldSquareRadiusRef, interactableDistanceRangeRef, canMoveEntityTag, moveCommandElementRef, entity) in
                SystemAPI.Query<
                    RefRO<AbsoluteDistanceXZToTarget>
                    , RefRO<TargetEntity>
                    , RefRO<TargetEntityWorldSquareRadius>
                    , RefRO<InteractableDistanceRange>
                    , EnabledRefRW<CanMoveEntityTag>
                    , RefRW<MoveCommandElement>>()
                    .WithDisabled<CanInteractEntityTag>()
                    .WithEntityAccess())
            {
                if (targetEntityRef.ValueRO.Value == Entity.Null) continue;

                float interactRadius = targetEntityWorldSquareRadiusRef.ValueRO.Value + interactableDistanceRangeRef.ValueRO.MaxValue;

                if (absoluteDistanceXZToTargetRef.ValueRO.X > interactRadius) continue;
                if (absoluteDistanceXZToTargetRef.ValueRO.Z > interactRadius) continue;

                this.StopMove(canMoveEntityTag, ref moveCommandElementRef.ValueRW);

                SystemAPI.SetComponentEnabled<CanInteractEntityTag>(entity, true);

            }

        }

        [BurstCompile]
        private void StopMove(
            EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
            , ref MoveCommandElement moveCommandElement)
        {
            canMoveEntityTag.ValueRW = false;
            moveCommandElement.CommandSource = MoveCommandSource.None;
            moveCommandElement.TargetEntity = Entity.Null;
        }

    }

}
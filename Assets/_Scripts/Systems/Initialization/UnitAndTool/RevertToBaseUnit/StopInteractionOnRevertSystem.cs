using Components.GameEntity.Interaction;
using Components.GameEntity.Movement.MoveCommand;
using Components.Unit.Misc;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Entities;
using Utilities.Helpers.Misc;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(RevertToBaseUnitSystemGroup))]
    [BurstCompile]
    public partial struct StopInteractionOnRevertSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InteractingEntity
                    , InteractionTypeICD>()
                .WithAll<
                    UnitTag
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (interactingEntityRef, interactionTypeICDRef, MoveCommandElementRef, TargetEntityRef) in SystemAPI
                .Query<
                    RefRW<InteractingEntity>
                    , RefRW<InteractionTypeICD>
                    , RefRO<MoveCommandElement>
                    , RefRO<TargetEntity>>()
                .WithAll<
                    UnitTag
                    , NeedRevertToBaseUnitTag>())
            {
                InteractionHelper.StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
            }

        }

    }

}

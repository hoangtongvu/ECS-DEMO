using Components.GameEntity.Interaction;
using Core.GameEntity;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.Misc
{
    [BurstCompile]
    public static class InteractionHelper
    {
        [BurstCompile]
        public static void StopInteraction(
            ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD)
        {
            interactingEntity.Value = Entity.Null;
            interactionTypeICD.Value = InteractionType.None;
        }

    }

}
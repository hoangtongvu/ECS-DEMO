using Components.GameEntity;
using Components.Unit.Misc;
using Components.Unit.MyMoveCommand;
using Core.GameEntity;
using Core.GameEntity.Misc;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.Misc
{
    [BurstCompile]
    public static class MoveCommandPrioritiesHelper
    {
        [BurstCompile]
        public static bool TryOverrideMoveCommand(
            in MoveCommandPrioritiesMap moveCommandPrioritiesMap
            , ref MoveCommandElement moveCommandElement
            , ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD
            , ArmedState armedState
            , MoveCommandSource newCommandSource)
        {
            byte currentPriority = GetCommandSourcePriority(moveCommandPrioritiesMap, armedState, moveCommandElement.CommandSource);
            byte newPriority = GetCommandSourcePriority(moveCommandPrioritiesMap, armedState, newCommandSource);

            // the lower the number is, the higher the priority
            if (currentPriority < newPriority) return false;

            moveCommandElement.CommandSource = newCommandSource;
            ResetInteraction(ref interactingEntity, ref interactionTypeICD);

            return true;
        }

        [BurstCompile]
        private static byte GetCommandSourcePriority(
            in MoveCommandPrioritiesMap moveCommandPrioritiesMap
            , ArmedState armedState
            , MoveCommandSource moveCommandSource)
        {
            int startIndex = GetStartIndexInMap(armedState);

            for (int i = startIndex; i < startIndex + MoveCommandSource_Length.Value; i++)
            {
                if (moveCommandPrioritiesMap.Value[i] != moveCommandSource) continue;
                return (byte)i;
            }

            return byte.MinValue;
        }

        [BurstCompile]
        private static void ResetInteraction(
            ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD)
        {
            interactingEntity.Value = Entity.Null;
            interactionTypeICD.Value = InteractionType.None;
        }

        [BurstCompile]
        public static int GetStartIndexInMap(ArmedState armedState) => ((byte)armedState) * MoveCommandSource_Length.Value;

    }

}
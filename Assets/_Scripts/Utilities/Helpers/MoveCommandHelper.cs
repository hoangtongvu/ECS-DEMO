using Components.Unit.MyMoveCommand;
using Core.Unit;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Collections;

namespace Utilities.Helpers
{
    [BurstCompile]
    public static class MoveCommandHelper
    {
        [BurstCompile]
        public static bool TryOverrideMoveCommand(
            in NativeHashMap<MoveCommandSourceId, byte> moveCommandSourceMap
            , UnitType unitType
            , ref MoveCommandElement moveCommandElement
            , MoveCommandSource newCommandSource
            , ushort localIndex)
        {
            var currentPriority = GetCommandSourcePriority(moveCommandSourceMap, unitType, moveCommandElement.CommandSource, localIndex);
            var newPriority = GetCommandSourcePriority(moveCommandSourceMap, unitType, newCommandSource, localIndex);

            // the lower the number is, the higher the priority
            if (currentPriority < newPriority) return false;

            moveCommandElement.CommandSource = newCommandSource;
            return true;
        }

        [BurstCompile]
        private static byte GetCommandSourcePriority(
            in NativeHashMap<MoveCommandSourceId, byte> moveCommandSourceMap
            , UnitType unitType
            , MoveCommandSource moveCommandSource
            , ushort localIndex)
        {
            var id = new MoveCommandSourceId(unitType, moveCommandSource, localIndex);
            return moveCommandSourceMap[id];
        }
    }
}
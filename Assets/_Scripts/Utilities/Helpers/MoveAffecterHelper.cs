using Components.Unit;
using Core.Unit;
using Unity.Burst;
using Unity.Collections;

namespace Utilities.Helpers
{
    public static class MoveAffecterHelper
    {
        [BurstCompile]
        public static bool TryChangeMoveAffecter(
            NativeHashMap<MoveAffecterId, byte> moveAffecterMap
            , UnitType unitType
            , ref MoveAffecterICD currentMoveAffecterICD
            , MoveAffecter newAffecter
            , ushort localIndex)
        {
            var currentPriority = GetMoveAffecterPriority(moveAffecterMap, unitType, currentMoveAffecterICD.Value, localIndex);
            var newPriority = GetMoveAffecterPriority(moveAffecterMap, unitType, newAffecter, localIndex);

            // the lower the number is, the higher the priority
            if (currentPriority < newPriority) return false;

            currentMoveAffecterICD.Value = newAffecter;
            return true;
        }

        [BurstCompile]
        private static byte GetMoveAffecterPriority(
            NativeHashMap<MoveAffecterId, byte> moveAffecterMap
            , UnitType unitType
            , MoveAffecter currentAffecter
            , ushort localIndex)
        {
            var id = new MoveAffecterId(unitType, currentAffecter, localIndex);
            return moveAffecterMap[id];
        }
    }
}
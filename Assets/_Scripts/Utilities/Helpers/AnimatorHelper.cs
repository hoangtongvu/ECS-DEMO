using Components;
using Unity.Entities;

namespace Utilities.Helpers
{
    public static class AnimatorHelper
    {
        public static bool TryChangeAnimatorData(RefRW<AnimatorData> animDataRef, string newAnimName)
        {
            if (animDataRef.ValueRO.AnimName == newAnimName) return false;
            animDataRef.ValueRW.AnimName = newAnimName;
            animDataRef.ValueRW.AnimChanged = true;
            return true;
        }
    }
}
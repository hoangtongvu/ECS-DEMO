using Core.Misc;
using Unity.Burst;


namespace Utilities.Extensions
{
    public static class ChangedFlagValueExtension
    {
        [BurstCompile]
        public static void ChangeValue<T>(ref this ChangedFlagValue<T> changedFlagValue, in T newValue) where T : unmanaged
        {
            changedFlagValue.Value = newValue;
            changedFlagValue.ValueChanged = true;
        }
    }
}

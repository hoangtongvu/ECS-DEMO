
namespace Components
{

    public struct ChangedFlagValue<T> where T : unmanaged
    {
        public T Value;
        public bool ValueChanged;
    }


    public static class ChangedFlagValueExtension
    {
        public static void ChangeValue<T>(ref this ChangedFlagValue<T> changedFlagValue, T newValue) where T : unmanaged
        {
            changedFlagValue.Value = newValue;
            changedFlagValue.ValueChanged = true;
        }
    }

}

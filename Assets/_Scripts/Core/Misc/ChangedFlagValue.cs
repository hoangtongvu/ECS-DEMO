

namespace Core.Misc
{

    public struct ChangedFlagValue<T> where T : unmanaged
    {
        public T Value;
        public bool ValueChanged;
    }
}

using Unity.Entities;

namespace Components.Misc
{
    /// <summary>
    /// Speed determines how many times unit can use their tool in 1 second.
    /// </summary>
    public struct BaseWorkSpeed : IComponentData
    {
        public float Value;
    }

    public struct WorkTimeCounterSecond : IComponentData
    {
        public float Value;
    }
}

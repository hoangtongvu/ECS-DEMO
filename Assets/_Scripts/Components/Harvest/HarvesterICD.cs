using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesterICD : IComponentData
    {
    }

    /// <summary>
    /// Speed determines how many times unit can use their tool in 1 second.
    /// </summary>
    public struct HarvestSpeed : IComponentData
    {
        public float Value;
    }

    public struct HarvestTimeCounterSecond : IComponentData
    {
        public float Value;
    }
}

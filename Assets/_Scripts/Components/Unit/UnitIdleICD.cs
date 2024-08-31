using Unity.Entities;

namespace Components.Unit
{
    public struct UnitIdleICD : IComponentData
    {
        public float TimeCounterSecond;
        public float TimeDurationSecond;
    }

}

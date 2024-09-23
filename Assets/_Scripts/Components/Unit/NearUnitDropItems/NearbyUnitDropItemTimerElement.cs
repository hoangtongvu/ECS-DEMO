using Unity.Entities;

namespace Components.Unit.NearUnitDropItems
{

    public struct NearbyUnitDropItemTimerElement : IBufferElementData
    {
        public Entity UnitEntity;
        public float CounterSecond;
    }

}

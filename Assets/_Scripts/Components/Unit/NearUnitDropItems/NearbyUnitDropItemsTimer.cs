using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace Components.Unit.NearUnitDropItems
{

    public struct NearbyUnitDropItemTimerElement : IBufferElementData
    {
        public Entity UnitEntity;
        public float CounterSecond;
    }

    public struct NearbyUnitDropItemTimeLimit : IComponentData
    {
        public float Value;
    }

    public struct NearbyUnitDistanceHitList : IComponentData
    {
        public NativeList<DistanceHit> Value;
    }

    public struct NearbyUnitHitRadius : IComponentData
    {
        public float Value;
    }

}

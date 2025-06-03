using Core.Unit;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit.Misc
{
    public struct DetectionRadiusMap : IComponentData
    {
        public NativeHashMap<UnitProfileId, half> Value;
    }

}
